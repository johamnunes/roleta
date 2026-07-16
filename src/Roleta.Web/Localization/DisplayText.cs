using Roleta.Domain.Catalog;
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

    public static string Label(this ModifierKind kind) => CanonicalModifiers.NameOf(kind);

    public static string Blurb(this ModifierKind kind) => kind switch
    {
        ModifierKind.Flip => "Vire uma regra",
        ModifierKind.Swap => "Uma sua ↔ uma dele",
        ModifierKind.Clone => "Copie do seu corpo",
        ModifierKind.Left => "Todos passam à esquerda",
        ModifierKind.Right => "Todos passam à direita",
        _ => string.Empty,
    };

    public static string CssClass(this ModifierKind kind) => kind switch
    {
        ModifierKind.Flip => "flip",
        ModifierKind.Swap => "swap",
        ModifierKind.Clone => "clone",
        ModifierKind.Left => "left",
        ModifierKind.Right => "right",
        _ => "flip",
    };

    public static string HexColor(this ModifierKind kind) => kind switch
    {
        ModifierKind.Flip => "#9b5de5",
        ModifierKind.Swap => "#e0a82e",
        ModifierKind.Clone => "#2ec4b6",
        ModifierKind.Left => "#4c6ef5",
        ModifierKind.Right => "#ff6b6b",
        _ => "#e0a82e",
    };
}
