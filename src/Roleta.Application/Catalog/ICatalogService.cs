using Roleta.Domain.Catalog;
using Roleta.Domain.Enums;

namespace Roleta.Application.Catalog;

/// <summary>
/// CRUD do catálogo editável de Regras, Modificadores e Ações.
/// O apresentador cadastra/altera esse conteúdo a qualquer momento, mesmo durante o show.
/// </summary>
public interface ICatalogService
{
    // ---- Regras ----
    Task<IReadOnlyList<RuleDefinition>> GetRulesAsync(
        Color? color = null, bool includeInactive = true, CancellationToken ct = default);
    Task<RuleDefinition?> GetRuleAsync(Guid id, CancellationToken ct = default);
    Task<RuleDefinition> SaveRuleAsync(RuleDefinition rule, CancellationToken ct = default);
    Task DeleteRuleAsync(Guid id, CancellationToken ct = default);

    // ---- Modificadores ----
    Task<IReadOnlyList<ModifierDefinition>> GetModifiersAsync(
        bool includeInactive = true, CancellationToken ct = default);
    Task<ModifierDefinition?> GetModifierAsync(Guid id, CancellationToken ct = default);
    Task<ModifierDefinition> SaveModifierAsync(ModifierDefinition modifier, CancellationToken ct = default);
    Task DeleteModifierAsync(Guid id, CancellationToken ct = default);

    // ---- Ações / Prompts ----
    Task<IReadOnlyList<ActionDefinition>> GetActionsAsync(
        bool includeInactive = true, CancellationToken ct = default);
    Task<ActionDefinition?> GetActionAsync(Guid id, CancellationToken ct = default);
    Task<ActionDefinition> SaveActionAsync(ActionDefinition action, CancellationToken ct = default);
    Task DeleteActionAsync(Guid id, CancellationToken ct = default);

    /// <summary>Contagens rápidas para a tela inicial.</summary>
    Task<CatalogCounts> GetCountsAsync(CancellationToken ct = default);
}

/// <summary>Resumo de quantidades do catálogo (total e ativos).</summary>
public readonly record struct CatalogCounts(
    int Rules, int RulesActive,
    int Modifiers, int ModifiersActive,
    int Actions, int ActionsActive);
