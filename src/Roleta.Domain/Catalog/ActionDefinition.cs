using System.ComponentModel.DataAnnotations;

namespace Roleta.Domain.Catalog;

/// <summary>
/// Definição de uma AÇÃO / PROMPT no catálogo (cadastrável/editável a qualquer momento).
/// Desafio imediato sorteado quando a roleta para numa seção do tipo Ação.
/// Ver docs/game-rules.md §4 e §10.
/// </summary>
public class ActionDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Texto do desafio, exibido na TV (PT-BR).</summary>
    [Required(ErrorMessage = "O texto da ação é obrigatório.")]
    [MaxLength(280)]
    public string Text { get; set; } = string.Empty;

    /// <summary>Duração sugerida em segundos (ex.: 15s). Opcional.</summary>
    [Range(1, 600, ErrorMessage = "A duração deve estar entre 1 e 600 segundos.")]
    public int? SuggestedDurationSeconds { get; set; }

    /// <summary>Pontos concedidos quando bem executada.</summary>
    [Range(0, 10, ErrorMessage = "A pontuação deve estar entre 0 e 10.")]
    public int ScoreOnSuccess { get; set; } = 1;

    /// <summary>Se false, não entra nos sorteios.</summary>
    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
