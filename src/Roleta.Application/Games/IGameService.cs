using Roleta.Domain.Games;

namespace Roleta.Application.Games;

/// <summary>CRUD dos jogos salvos (nome, jogadores e conteúdo selecionado).</summary>
public interface IGameService
{
    /// <summary>Lista os jogos (sem carregar o conteúdo) para exibição/seleção.</summary>
    Task<IReadOnlyList<Game>> GetGamesAsync(CancellationToken ct = default);

    /// <summary>Carrega um jogo COM o conteúdo (regras/modificadores/ações).</summary>
    Task<Game?> GetGameAsync(Guid id, CancellationToken ct = default);

    /// <summary>Cria/atualiza um jogo, definindo o conteúdo pelos ids selecionados.</summary>
    Task<Game> SaveGameAsync(
        Game game,
        IReadOnlyCollection<Guid> ruleIds,
        IReadOnlyCollection<Guid> modifierIds,
        IReadOnlyCollection<Guid> actionIds,
        CancellationToken ct = default);

    Task DeleteGameAsync(Guid id, CancellationToken ct = default);
}
