using Microsoft.EntityFrameworkCore;
using Roleta.Application.Games;
using Roleta.Domain.Games;

namespace Roleta.Infrastructure.Persistence;

/// <summary>Implementação EF Core/SQLite do <see cref="IGameService"/>.</summary>
public sealed class GameService(IDbContextFactory<RoletaDbContext> factory) : IGameService
{
    public async Task<IReadOnlyList<Game>> GetGamesAsync(CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Games.AsNoTracking().OrderBy(g => g.Name).ToListAsync(ct);
    }

    public async Task<Game?> GetGameAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Games
            .AsNoTracking()
            .Include(g => g.Rules)
            .Include(g => g.Modifiers)
            .Include(g => g.Actions)
            .FirstOrDefaultAsync(g => g.Id == id, ct);
    }

    public async Task<Game> SaveGameAsync(
        Game game,
        IReadOnlyCollection<Guid> ruleIds,
        IReadOnlyCollection<Guid> modifierIds,
        IReadOnlyCollection<Guid> actionIds,
        CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        // Itens do catálogo já rastreados neste contexto (para criar só as junções, sem reinserir).
        var rules = await db.Rules.Where(r => ruleIds.Contains(r.Id)).ToListAsync(ct);
        var modifiers = await db.Modifiers.Where(m => modifierIds.Contains(m.Id)).ToListAsync(ct);
        var actions = await db.Actions.Where(a => actionIds.Contains(a.Id)).ToListAsync(ct);

        var existing = await db.Games
            .Include(g => g.Rules)
            .Include(g => g.Modifiers)
            .Include(g => g.Actions)
            .FirstOrDefaultAsync(g => g.Id == game.Id, ct);

        if (existing is null)
        {
            game.Rules = rules;
            game.Modifiers = modifiers;
            game.Actions = actions;
            game.UpdatedAt = DateTimeOffset.UtcNow;
            db.Games.Add(game);
            await db.SaveChangesAsync(ct);
            return game;
        }

        existing.Name = game.Name;
        existing.Player1 = game.Player1;
        existing.Player2 = game.Player2;
        existing.Player3 = game.Player3;
        existing.EndingScript = game.EndingScript;
        existing.BasePoints = game.BasePoints;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        existing.Rules.Clear();
        existing.Rules.AddRange(rules);
        existing.Modifiers.Clear();
        existing.Modifiers.AddRange(modifiers);
        existing.Actions.Clear();
        existing.Actions.AddRange(actions);

        await db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task DeleteGameAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var game = await db.Games
            .Include(g => g.Rules)
            .Include(g => g.Modifiers)
            .Include(g => g.Actions)
            .FirstOrDefaultAsync(g => g.Id == id, ct);
        if (game is null) return;
        db.Games.Remove(game); // remove o jogo e as linhas das tabelas de junção
        await db.SaveChangesAsync(ct);
    }
}
