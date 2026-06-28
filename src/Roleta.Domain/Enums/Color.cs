namespace Roleta.Domain.Enums;

/// <summary>
/// Cores/pools temáticos da roleta física. Cada cor é um grupo de regras.
/// Ver docs/game-rules.md §3.
/// </summary>
public enum Color
{
    /// <summary>Azul — regras de fala / verbais ("BOCA").</summary>
    Blue = 0,

    /// <summary>Vermelho — regras físicas / corporais ("CORPO").</summary>
    Red = 1,

    /// <summary>Verde — regras de interação / relação ("GENTE").</summary>
    Green = 2,

    /// <summary>
    /// Branca — a PLATEIA completa a regra: uma frase pré-escrita com uma lacuna
    /// que o público preenche ao vivo.
    /// </summary>
    White = 3,
}
