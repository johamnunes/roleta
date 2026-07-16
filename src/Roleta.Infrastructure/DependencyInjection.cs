using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roleta.Application.Catalog;
using Roleta.Application.Games;
using Roleta.Infrastructure.Catalog;
using Roleta.Infrastructure.Persistence;

namespace Roleta.Infrastructure;

/// <summary>Registro de DI e inicialização da camada de infraestrutura.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra o SQLite (via DbContextFactory) e os serviços de catálogo.
    /// </summary>
    /// <param name="dbPath">Caminho absoluto do arquivo .db (ex.: ./data/roleta.db).</param>
    public static IServiceCollection AddRoletaInfrastructure(this IServiceCollection services, string dbPath)
    {
        services.AddDbContextFactory<RoletaDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IGameService, GameService>();
        return services;
    }

    /// <summary>
    /// Aplica migrations, garante WAL e popula os modificadores canônicos.
    /// Chamar uma vez no startup, antes de servir requisições.
    /// </summary>
    public static async Task InitializeRoletaDatabaseAsync(
        this IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<RoletaDbContext>>();
        await using var db = await factory.CreateDbContextAsync(ct);

        await db.Database.MigrateAsync(ct);
        // WAL persiste no arquivo após definido uma vez; melhora escrita e resistência a crash.
        await db.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;", ct);
        await ModifierCatalogSeeder.EnsureCanonicalModifiersAsync(db, ct);
    }
}
