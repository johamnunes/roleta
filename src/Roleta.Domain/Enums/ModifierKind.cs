namespace Roleta.Domain.Enums;

/// <summary>
/// Tipo canônico de modificador. O nome na TV fica em <c>ModifierDefinition.Name</c> (PT-BR);
/// o kind identifica o efeito e o card visual.
/// Ver docs/game-rules.md §8.
/// </summary>
public enum ModifierKind
{
    /// <summary>Inversão — vira uma regra no corpo (base ↔ oposto).</summary>
    Flip = 0,

    /// <summary>Troca — troca mútua de uma regra sua com a de outro participante.</summary>
    Swap = 1,

    /// <summary>CTRL+C CTRL+V — copia uma regra do seu corpo para outro participante.</summary>
    Clone = 2,

    /// <summary>ESQUERDA — cada um move uma regra para o jogador à esquerda.</summary>
    Left = 3,

    /// <summary>DIREITA — cada um move uma regra para o jogador à direita.</summary>
    Right = 4,
}
