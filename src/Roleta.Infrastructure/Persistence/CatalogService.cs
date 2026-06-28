using Microsoft.EntityFrameworkCore;
using Roleta.Application.Catalog;
using Roleta.Domain.Catalog;
using Roleta.Domain.Enums;

namespace Roleta.Infrastructure.Persistence;

/// <summary>
/// Implementação EF Core/SQLite do <see cref="ICatalogService"/>.
/// Usa <see cref="IDbContextFactory{TContext}"/> para criar um contexto curto por operação,
/// adequado a um consumidor de vida longa (o app fica aberto o show inteiro).
/// </summary>
public sealed class CatalogService(IDbContextFactory<RoletaDbContext> factory) : ICatalogService
{
    // ---- Regras ----
    public async Task<IReadOnlyList<RuleDefinition>> GetRulesAsync(
        Color? color = null, bool includeInactive = true, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var query = db.Rules.AsNoTracking().AsQueryable();
        if (color is not null) query = query.Where(r => r.Color == color);
        if (!includeInactive) query = query.Where(r => r.IsActive);
        return await query.OrderBy(r => r.Color).ThenBy(r => r.Text).ToListAsync(ct);
    }

    public async Task<RuleDefinition?> GetRuleAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Rules.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<RuleDefinition> SaveRuleAsync(RuleDefinition rule, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        rule.UpdatedAt = DateTimeOffset.UtcNow;
        var exists = await db.Rules.AnyAsync(r => r.Id == rule.Id, ct);
        if (exists) db.Rules.Update(rule);
        else db.Rules.Add(rule);
        await db.SaveChangesAsync(ct);
        return rule;
    }

    public async Task DeleteRuleAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        await db.Rules.Where(r => r.Id == id).ExecuteDeleteAsync(ct);
    }

    // ---- Modificadores ----
    public async Task<IReadOnlyList<ModifierDefinition>> GetModifiersAsync(
        bool includeInactive = true, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var query = db.Modifiers.AsNoTracking().AsQueryable();
        if (!includeInactive) query = query.Where(m => m.IsActive);
        return await query.OrderBy(m => m.Name).ToListAsync(ct);
    }

    public async Task<ModifierDefinition?> GetModifierAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Modifiers.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    public async Task<ModifierDefinition> SaveModifierAsync(ModifierDefinition modifier, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        modifier.UpdatedAt = DateTimeOffset.UtcNow;
        var exists = await db.Modifiers.AnyAsync(m => m.Id == modifier.Id, ct);
        if (exists) db.Modifiers.Update(modifier);
        else db.Modifiers.Add(modifier);
        await db.SaveChangesAsync(ct);
        return modifier;
    }

    public async Task DeleteModifierAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        await db.Modifiers.Where(m => m.Id == id).ExecuteDeleteAsync(ct);
    }

    // ---- Ações ----
    public async Task<IReadOnlyList<ActionDefinition>> GetActionsAsync(
        bool includeInactive = true, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var query = db.Actions.AsNoTracking().AsQueryable();
        if (!includeInactive) query = query.Where(a => a.IsActive);
        return await query.OrderBy(a => a.Text).ToListAsync(ct);
    }

    public async Task<ActionDefinition?> GetActionAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Actions.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<ActionDefinition> SaveActionAsync(ActionDefinition action, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        action.UpdatedAt = DateTimeOffset.UtcNow;
        var exists = await db.Actions.AnyAsync(a => a.Id == action.Id, ct);
        if (exists) db.Actions.Update(action);
        else db.Actions.Add(action);
        await db.SaveChangesAsync(ct);
        return action;
    }

    public async Task DeleteActionAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        await db.Actions.Where(a => a.Id == id).ExecuteDeleteAsync(ct);
    }

    // ---- Resumo ----
    public async Task<CatalogCounts> GetCountsAsync(CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return new CatalogCounts(
            Rules: await db.Rules.CountAsync(ct),
            RulesActive: await db.Rules.CountAsync(r => r.IsActive, ct),
            Modifiers: await db.Modifiers.CountAsync(ct),
            ModifiersActive: await db.Modifiers.CountAsync(m => m.IsActive, ct),
            Actions: await db.Actions.CountAsync(ct),
            ActionsActive: await db.Actions.CountAsync(a => a.IsActive, ct));
    }
}
