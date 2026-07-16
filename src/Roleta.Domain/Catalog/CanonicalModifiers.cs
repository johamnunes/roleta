using Roleta.Domain.Enums;

namespace Roleta.Domain.Catalog;

/// <summary>Nomes canônicos (PT-BR) dos 5 modificadores do show.</summary>
public static class CanonicalModifiers
{
    /// <summary>Cópias de cada kind na sacola de sorteio (sem reposição).</summary>
    public const int CopiesPerKind = 2;

    public static IReadOnlyList<(ModifierKind Kind, string Name)> All { get; } =
    [
        (ModifierKind.Flip, "Inversão"),
        (ModifierKind.Swap, "Troca"),
        (ModifierKind.Clone, "CTRL+C CTRL+V"),
        (ModifierKind.Left, "ESQUERDA"),
        (ModifierKind.Right, "DIREITA"),
    ];

    public static string NameOf(ModifierKind kind) =>
        All.First(x => x.Kind == kind).Name;
}
