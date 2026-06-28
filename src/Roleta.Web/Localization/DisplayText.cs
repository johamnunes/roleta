using Roleta.Domain.Enums;

namespace Roleta.Web.Localization;

/// <summary>
/// Rótulos PT-BR para os enums (que têm identificadores em inglês).
/// Toda a parte voltada ao usuário aparece em português.
/// </summary>
public static class DisplayText
{
    public static string Label(this Color color) => color switch
    {
        Color.Blue => "Azul",
        Color.Red => "Vermelho",
        Color.Green => "Verde",
        Color.White => "Branca",
        _ => color.ToString(),
    };

    public static string Theme(this Color color) => color switch
    {
        Color.Blue => "Boca — fala",
        Color.Red => "Corpo — físico",
        Color.Green => "Gente — interação",
        Color.White => "Plateia completa a regra",
        _ => string.Empty,
    };

    /// <summary>Classe de badge do Bootstrap por cor.</summary>
    public static string BadgeClass(this Color color) => color switch
    {
        Color.Blue => "text-bg-primary",
        Color.Red => "text-bg-danger",
        Color.Green => "text-bg-success",
        Color.White => "text-bg-light border",
        _ => "text-bg-secondary",
    };

    /// <summary>Cor hex para destaque no telão.</summary>
    public static string HexColor(this Color color) => color switch
    {
        Color.Blue => "#1b6ec2",
        Color.Red => "#e10600",
        Color.Green => "#198754",
        Color.White => "#f8f9fa",
        _ => "#888888",
    };
}
