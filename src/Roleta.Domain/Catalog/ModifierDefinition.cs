using System.ComponentModel.DataAnnotations;
using Roleta.Domain.Enums;

namespace Roleta.Domain.Catalog;

/// <summary>
/// Definição de um MODIFICADOR no catálogo (cadastrável/editável a qualquer momento).
/// <see cref="Kind"/> identifica o efeito; <see cref="Name"/> é o texto da TV.
/// Ver docs/game-rules.md §8.
/// </summary>
public class ModifierDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Tipo canônico do modificador (efeito + card visual).</summary>
    public ModifierKind Kind { get; set; }

    /// <summary>Nome do modificador exibido na TV (ex.: "Inversão").</summary>
    [Required(ErrorMessage = "O nome do modificador é obrigatório.")]
    [MaxLength(60)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Se false, não entra nos sorteios.</summary>
    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
