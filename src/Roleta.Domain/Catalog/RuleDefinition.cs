using System.ComponentModel.DataAnnotations;
using Roleta.Domain.Enums;

namespace Roleta.Domain.Catalog;

/// <summary>
/// Definição de uma REGRA no catálogo (cadastrável/editável a qualquer momento).
/// É o conteúdo sorteado quando a roleta para numa seção do tipo Regra.
/// Ver docs/game-rules.md §4 e §10.
/// </summary>
public class RuleDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Cor/pool a que esta regra pertence.</summary>
    public Color Color { get; set; }

    /// <summary>Texto da regra exibido na TV (PT-BR).</summary>
    [Required(ErrorMessage = "O texto da regra é obrigatório.")]
    [MaxLength(280)]
    public string Text { get; set; } = string.Empty;

    /// <summary>Texto do oposto, usado pelo modificador FLIP. Opcional.</summary>
    [MaxLength(280)]
    public string? OppositeText { get; set; }

    /// <summary>Notas internas do juiz sobre a regra BASE (não vão para a TV).</summary>
    [MaxLength(280)]
    public string? JudgeNotes { get; set; }

    /// <summary>Notas internas do juiz sobre a regra INVERTIDA/oposto (não vão para a TV).</summary>
    [MaxLength(280)]
    public string? OppositeJudgeNotes { get; set; }

    /// <summary>Se false, a regra não entra nos sorteios (desativada sem apagar).</summary>
    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
