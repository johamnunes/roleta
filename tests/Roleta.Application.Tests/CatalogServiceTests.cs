using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Roleta.Domain.Catalog;
using Roleta.Domain.Enums;
using Roleta.Infrastructure.Persistence;

namespace Roleta.Application.Tests;

public class CatalogServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly TestDbContextFactory _factory;
    private readonly CatalogService _service;

    public CatalogServiceTests()
    {
        // SQLite in-memory: a conexão precisa ficar aberta durante o teste.
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<RoletaDbContext>()
            .UseSqlite(_connection)
            .Options;

        using (var ctx = new RoletaDbContext(options))
        {
            ctx.Database.EnsureCreated();
        }

        _factory = new TestDbContextFactory(options);
        _service = new CatalogService(_factory);
    }

    [Fact]
    public async Task Catalog_starts_empty()
    {
        var counts = await _service.GetCountsAsync();

        Assert.Equal(0, counts.Rules);
        Assert.Equal(0, counts.Modifiers);
        Assert.Equal(0, counts.Actions);
    }

    [Fact]
    public async Task SaveRule_inserts_then_returns_in_list()
    {
        var rule = new RuleDefinition { Color = Color.Blue, Text = "Só fale no diminutivo." };

        await _service.SaveRuleAsync(rule);

        var all = await _service.GetRulesAsync();
        var stored = Assert.Single(all);
        Assert.Equal("Só fale no diminutivo.", stored.Text);
        Assert.Equal(Color.Blue, stored.Color);
    }

    [Fact]
    public async Task SaveRule_updates_existing_without_duplicating()
    {
        var rule = new RuleDefinition { Color = Color.Red, Text = "Original" };
        await _service.SaveRuleAsync(rule);

        rule.Text = "Editado";
        await _service.SaveRuleAsync(rule);

        var all = await _service.GetRulesAsync();
        var stored = Assert.Single(all);
        Assert.Equal("Editado", stored.Text);
    }

    [Fact]
    public async Task DeleteRule_removes_it()
    {
        var rule = new RuleDefinition { Color = Color.Green, Text = "Concorde com tudo." };
        await _service.SaveRuleAsync(rule);

        await _service.DeleteRuleAsync(rule.Id);

        Assert.Empty(await _service.GetRulesAsync());
    }

    [Fact]
    public async Task GetRules_filters_by_color()
    {
        await _service.SaveRuleAsync(new RuleDefinition { Color = Color.Blue, Text = "Azul" });
        await _service.SaveRuleAsync(new RuleDefinition { Color = Color.White, Text = "A plateia escolhe: ______" });

        var white = await _service.GetRulesAsync(Color.White);

        var only = Assert.Single(white);
        Assert.Equal(Color.White, only.Color);
    }

    [Fact]
    public async Task GetRules_can_exclude_inactive()
    {
        await _service.SaveRuleAsync(new RuleDefinition { Color = Color.Blue, Text = "Ativa", IsActive = true });
        await _service.SaveRuleAsync(new RuleDefinition { Color = Color.Blue, Text = "Inativa", IsActive = false });

        var actives = await _service.GetRulesAsync(includeInactive: false);

        var only = Assert.Single(actives);
        Assert.Equal("Ativa", only.Text);
    }

    [Fact]
    public async Task SaveModifier_and_SaveAction_persist()
    {
        await _service.SaveModifierAsync(new ModifierDefinition
        {
            Kind = ModifierKind.Swap,
            Name = CanonicalModifiers.NameOf(ModifierKind.Swap),
        });
        await _service.SaveActionAsync(new ActionDefinition { Text = "Faça um discurso de 15s." });

        var counts = await _service.GetCountsAsync();
        Assert.Equal(1, counts.Modifiers);
        Assert.Equal(1, counts.Actions);
        var mod = Assert.Single(await _service.GetModifiersAsync());
        Assert.Equal(ModifierKind.Swap, mod.Kind);
        Assert.Equal("Troca", mod.Name);
    }

    public void Dispose() => _connection.Dispose();

    /// <summary>Fábrica de contexto que reutiliza a mesma conexão in-memory.</summary>
    private sealed class TestDbContextFactory(DbContextOptions<RoletaDbContext> options)
        : IDbContextFactory<RoletaDbContext>
    {
        public RoletaDbContext CreateDbContext() => new(options);
    }
}
