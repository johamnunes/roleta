using Microsoft.EntityFrameworkCore;
using Roleta.Domain.Catalog;
using Roleta.Domain.Games;

namespace Roleta.Infrastructure.Persistence;

/// <summary>
/// Contexto EF Core do app: catálogo editável (Regras, Modificadores, Ações) e os Jogos salvos.
/// </summary>
public class RoletaDbContext(DbContextOptions<RoletaDbContext> options) : DbContext(options)
{
    public DbSet<RuleDefinition> Rules => Set<RuleDefinition>();
    public DbSet<ModifierDefinition> Modifiers => Set<ModifierDefinition>();
    public DbSet<ActionDefinition> Actions => Set<ActionDefinition>();
    public DbSet<Game> Games => Set<Game>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RuleDefinition>(e =>
        {
            e.HasKey(r => r.Id);
            // Enum como texto: legível ao inspecionar o .db e estável a reordenações.
            e.Property(r => r.Color).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(r => r.Color);
        });

        modelBuilder.Entity<ModifierDefinition>(e =>
        {
            e.HasKey(m => m.Id);
        });

        modelBuilder.Entity<ActionDefinition>(e =>
        {
            e.HasKey(a => a.Id);
        });

        modelBuilder.Entity<Game>(e =>
        {
            e.HasKey(g => g.Id);
            // muitos-para-muitos unidirecional com o catálogo (EF cria as tabelas de junção)
            e.HasMany(g => g.Rules).WithMany();
            e.HasMany(g => g.Modifiers).WithMany();
            e.HasMany(g => g.Actions).WithMany();
        });
    }
}
