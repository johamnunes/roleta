using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Roleta.Domain.Catalog;
using Roleta.Domain.Enums;
using Roleta.Infrastructure.Catalog;
using Roleta.Infrastructure.Persistence;

namespace Roleta.Application.Tests;

public class ModifierCatalogSeederTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<RoletaDbContext> _options;

    public ModifierCatalogSeederTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<RoletaDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var ctx = new RoletaDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    [Fact]
    public async Task EnsureCanonical_creates_five_with_canonical_names()
    {
        await using (var db = new RoletaDbContext(_options))
            await ModifierCatalogSeeder.EnsureCanonicalModifiersAsync(db);

        await using var read = new RoletaDbContext(_options);
        var mods = await read.Modifiers.Where(m => m.IsActive).OrderBy(m => m.Kind).ToListAsync();

        Assert.Equal(CanonicalModifiers.All.Count, mods.Count);
        foreach (var (kind, name) in CanonicalModifiers.All)
        {
            var m = Assert.Single(mods, x => x.Kind == kind);
            Assert.Equal(name, m.Name);
            Assert.True(m.IsActive);
        }
    }

    [Fact]
    public async Task EnsureCanonical_is_idempotent_and_renames()
    {
        await using (var db = new RoletaDbContext(_options))
        {
            db.Modifiers.Add(new ModifierDefinition { Kind = ModifierKind.Swap, Name = "SWAP", IsActive = true });
            await db.SaveChangesAsync();
            await ModifierCatalogSeeder.EnsureCanonicalModifiersAsync(db);
            await ModifierCatalogSeeder.EnsureCanonicalModifiersAsync(db);
        }

        await using var read = new RoletaDbContext(_options);
        var active = await read.Modifiers.Where(m => m.IsActive).ToListAsync();
        Assert.Equal(CanonicalModifiers.All.Count, active.Count);
        Assert.Equal("Troca", Assert.Single(active, m => m.Kind == ModifierKind.Swap).Name);
    }

    public void Dispose() => _connection.Dispose();
}
