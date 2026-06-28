using System.ComponentModel.DataAnnotations;
using Roleta.Domain.Catalog;

namespace Roleta.Domain.Games;

/// <summary>
/// Um "jogo" salvo: nome, os 3 jogadores e quais regras/modificadores/ações entram nele.
/// Durante a partida, regras e ações só podem sair UMA vez (sem repetição); modificadores podem repetir.
/// </summary>
public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "O nome do jogo é obrigatório.")]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(40)] public string Player1 { get; set; } = "Jogador 1";
    [MaxLength(40)] public string Player2 { get; set; } = "Jogador 2";
    [MaxLength(40)] public string Player3 { get; set; } = "Jogador 3";

    // Conteúdo selecionado para este jogo (muitos-para-muitos com o catálogo).
    public List<RuleDefinition> Rules { get; set; } = [];
    public List<ModifierDefinition> Modifiers { get; set; } = [];
    public List<ActionDefinition> Actions { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
