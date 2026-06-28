using System.ComponentModel.DataAnnotations;

namespace Roleta.Domain.Catalog;

/// <summary>
/// Definição de um MODIFICADOR no catálogo (cadastrável/editável a qualquer momento).
/// Só precisa de um nome — o efeito é conhecido no palco (ex.: SWAP, CLONE, FLIP).
/// Ver docs/game-rules.md §8.
/// </summary>
public class ModifierDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Nome do modificador exibido na TV (ex.: "SWAP").</summary>
    [Required(ErrorMessage = "O nome do modificador é obrigatório.")]
    [MaxLength(60)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Se false, não entra nos sorteios.</summary>
    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
