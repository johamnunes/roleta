using Roleta.Domain.Catalog;
using Roleta.Domain.Enums;
using Roleta.Domain.Games;

namespace Roleta.Application.Show;

public enum RevealKind { Rule, Modifier, Action }

/// <summary>O que está atualmente no quadro grande do telão.</summary>
public sealed record Reveal(RevealKind Kind, string Title, string Text, Color? Color);

public sealed class ShowPlayer
{
    public string Name { get; set; } = string.Empty;
    public int Score { get; set; }
}

/// <summary>
/// Estado vivo do show, compartilhado entre as telas de Controle e Público (singleton em memória).
/// Qualquer mutação dispara <see cref="Changed"/>, e ambas as telas se re-renderizam via SignalR.
/// (Persistência/resume em SQLite virá numa fase futura — ver docs/game-rules.md anexo.)
/// </summary>
public sealed class ShowState : IDisposable
{
    private readonly List<ShowPlayer> _players =
    [
        new() { Name = "Jogador 1" },
        new() { Name = "Jogador 2" },
        new() { Name = "Jogador 3" },
    ];

    private readonly Dictionary<string, Guid> _lastByPool = [];

    // ---- Cronômetro (contagem regressiva da ação) ----
    private readonly System.Threading.Timer _ticker;
    private readonly object _timerLock = new();

    public int TimerTotalSeconds { get; private set; }
    public int TimerRemainingSeconds { get; private set; }
    public bool TimerRunning { get; private set; }

    public ShowState()
    {
        _ticker = new System.Threading.Timer(_ => Tick(), null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>Inicia (ou reinicia) a contagem regressiva com a duração dada (ex.: 30 ou 60s).</summary>
    public void StartTimer(int seconds)
    {
        lock (_timerLock)
        {
            TimerTotalSeconds = seconds;
            TimerRemainingSeconds = seconds;
            TimerRunning = seconds > 0;
            _ticker.Change(1000, 1000);
        }
        // ao ativar o cronômetro, limpa o card que estiver no telão
        Current = null;
        IsFlipReveal = false;
        CurrentRule = null;
        IsShowingOpposite = false;
        Changed?.Invoke();
    }

    public void PauseTimer()
    {
        lock (_timerLock)
        {
            if (!TimerRunning) return;
            TimerRunning = false;
            _ticker.Change(Timeout.Infinite, Timeout.Infinite);
        }
        Changed?.Invoke();
    }

    public void ResumeTimer()
    {
        lock (_timerLock)
        {
            if (TimerRunning || TimerRemainingSeconds <= 0) return;
            TimerRunning = true;
            _ticker.Change(1000, 1000);
        }
        Changed?.Invoke();
    }

    /// <summary>Zera e esconde o cronômetro.</summary>
    public void StopTimer()
    {
        lock (_timerLock)
        {
            TimerRunning = false;
            TimerTotalSeconds = 0;
            TimerRemainingSeconds = 0;
            _ticker.Change(Timeout.Infinite, Timeout.Infinite);
        }
        Changed?.Invoke();
    }

    public void AddTimerSeconds(int delta)
    {
        lock (_timerLock)
        {
            if (TimerTotalSeconds == 0) return;
            TimerRemainingSeconds = Math.Max(0, TimerRemainingSeconds + delta);
            if (TimerRemainingSeconds > TimerTotalSeconds) TimerTotalSeconds = TimerRemainingSeconds;
        }
        Changed?.Invoke();
    }

    private void Tick()
    {
        lock (_timerLock)
        {
            if (!TimerRunning) return;
            TimerRemainingSeconds--;
            if (TimerRemainingSeconds <= 0)
            {
                TimerRemainingSeconds = 0;
                TimerRunning = false;
                _ticker.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
        Changed?.Invoke();
    }

    public void Dispose() => _ticker.Dispose();

    public IReadOnlyList<ShowPlayer> Players => _players;
    public Reveal? Current { get; private set; }
    public bool DisplayVisible { get; private set; } = true;

    /// <summary>Incrementa a cada nova revelação — usado como key para reiniciar a animação no telão.</summary>
    public int RevealSeq { get; private set; }

    /// <summary>Animação da revelação atual: "virar o card" (true) ou "abrir" (false).</summary>
    public bool IsFlipReveal { get; private set; }

    /// <summary>Se o texto mostrado é o OPOSTO da regra (define a nota e o toggle do Inverter).</summary>
    public bool IsShowingOpposite { get; private set; }

    /// <summary>Regra mostrada no momento (para o "Inverter" alternar base/oposto e exibir a nota).</summary>
    public RuleDefinition? CurrentRule { get; private set; }

    private readonly List<RuleDefinition> _ruleHistory = [];

    /// <summary>Regras que já saíram nesta sessão (mais recente primeiro).</summary>
    public IReadOnlyList<RuleDefinition> RuleHistory => _ruleHistory;

    // ---- Jogo carregado ----
    private readonly List<RuleDefinition> _gameRules = [];
    private readonly List<ModifierDefinition> _gameModifiers = [];
    private readonly List<ActionDefinition> _gameActions = [];
    private readonly HashSet<Guid> _usedRuleIds = [];
    private readonly HashSet<Guid> _usedActionIds = [];

    /// <summary>Nome do jogo carregado, ou null se está em modo livre (sorteia do catálogo inteiro).</summary>
    public string? ActiveGameName { get; private set; }
    public bool HasActiveGame => ActiveGameName is not null;

    public int RemainingRules(Color color) =>
        _gameRules.Count(r => r.Color == color && !_usedRuleIds.Contains(r.Id));
    public int RemainingActions => _gameActions.Count(a => !_usedActionIds.Contains(a.Id));
    public int RemainingModifiers => _gameModifiers.Count;

    /// <summary>Disparado a cada mutação do estado.</summary>
    public event Action? Changed;

    public void SetReveal(Reveal reveal)
    {
        Current = reveal;
        DisplayVisible = true;
        IsFlipReveal = false;
        CurrentRule = null;
        IsShowingOpposite = false;
        RevealSeq++;
        Changed?.Invoke();
    }

    /// <summary>
    /// Mostra uma regra no telão (e a registra no histórico). Com <paramref name="flipped"/>,
    /// mostra o oposto da regra com animação de virar o card.
    /// </summary>
    private void RevealRuleInternal(RuleDefinition rule, bool opposite, bool flipAnimation)
    {
        var text = opposite && !string.IsNullOrWhiteSpace(rule.OppositeText)
            ? rule.OppositeText!
            : rule.Text;

        Current = new Reveal(RevealKind.Rule, string.Empty, text, rule.Color);
        DisplayVisible = true;
        IsFlipReveal = flipAnimation;
        IsShowingOpposite = opposite;
        CurrentRule = rule;
        RevealSeq++;

        // histórico: mais recente no topo, sem duplicar
        _ruleHistory.RemoveAll(r => r.Id == rule.Id);
        _ruleHistory.Insert(0, rule);

        Changed?.Invoke();
    }

    /// <summary>Mostra a regra (texto base) com a animação de ABRIR. Usado por revelar/Mostrar.</summary>
    public void ShowRule(RuleDefinition rule) =>
        RevealRuleInternal(rule, opposite: false, flipAnimation: false);

    /// <summary>
    /// Inverter: alterna entre base e oposto SEMPRE com a animação de VIRAR o card.
    /// Se já está no oposto desta regra, volta à base (também virando).
    /// </summary>
    public void ToggleRule(RuleDefinition rule)
    {
        var currentlyOpposite = CurrentRule?.Id == rule.Id && IsShowingOpposite;
        RevealRuleInternal(rule, opposite: !currentlyOpposite, flipAnimation: true);
    }

    /// <summary>
    /// Carrega um jogo: aplica os nomes dos jogadores, define os pools de conteúdo,
    /// zera o placar, o histórico e os itens "já usados".
    /// </summary>
    public void LoadGame(Game game)
    {
        ActiveGameName = game.Name;

        _players[0].Name = string.IsNullOrWhiteSpace(game.Player1) ? "Jogador 1" : game.Player1;
        _players[1].Name = string.IsNullOrWhiteSpace(game.Player2) ? "Jogador 2" : game.Player2;
        _players[2].Name = string.IsNullOrWhiteSpace(game.Player3) ? "Jogador 3" : game.Player3;
        foreach (var p in _players) p.Score = 0;

        _gameRules.Clear(); _gameRules.AddRange(game.Rules);
        _gameModifiers.Clear(); _gameModifiers.AddRange(game.Modifiers);
        _gameActions.Clear(); _gameActions.AddRange(game.Actions);
        _usedRuleIds.Clear();
        _usedActionIds.Clear();

        _ruleHistory.Clear();
        Current = null;
        IsFlipReveal = false;
        CurrentRule = null;
        IsShowingOpposite = false;
        Changed?.Invoke();
    }

    /// <summary>Volta ao modo livre (sorteia do catálogo inteiro, sem controle de repetição).</summary>
    public void ClearGame()
    {
        ActiveGameName = null;
        _gameRules.Clear();
        _gameModifiers.Clear();
        _gameActions.Clear();
        _usedRuleIds.Clear();
        _usedActionIds.Clear();
        Changed?.Invoke();
    }

    /// <summary>Sorteia uma regra da cor no jogo carregado, sem repetir. false se acabaram.</summary>
    public bool DrawRule(Color color)
    {
        var candidates = _gameRules.Where(r => r.Color == color && !_usedRuleIds.Contains(r.Id)).ToList();
        if (candidates.Count == 0) return false;
        var rule = candidates[Random.Shared.Next(candidates.Count)];
        _usedRuleIds.Add(rule.Id);
        ShowRule(rule);
        return true;
    }

    /// <summary>Sorteia uma ação do jogo carregado, sem repetir. false se acabaram.</summary>
    public bool DrawAction()
    {
        var candidates = _gameActions.Where(a => !_usedActionIds.Contains(a.Id)).ToList();
        if (candidates.Count == 0) return false;
        var action = candidates[Random.Shared.Next(candidates.Count)];
        _usedActionIds.Add(action.Id);
        SetReveal(new Reveal(RevealKind.Action, string.Empty, action.Text, null));
        return true;
    }

    /// <summary>Sorteia um modificador do jogo carregado (pode repetir). false se não há nenhum.</summary>
    public bool DrawModifier()
    {
        if (_gameModifiers.Count == 0) return false;
        var modifier = _gameModifiers[Random.Shared.Next(_gameModifiers.Count)];
        SetReveal(new Reveal(RevealKind.Modifier, string.Empty, modifier.Name, null));
        return true;
    }

    public void ClearReveal()
    {
        Current = null;
        IsFlipReveal = false;
        CurrentRule = null;
        IsShowingOpposite = false;
        Changed?.Invoke();
    }

    public void SetDisplayVisible(bool visible)
    {
        DisplayVisible = visible;
        Changed?.Invoke();
    }

    public void SetPlayerName(int index, string name)
    {
        if (index < 0 || index >= _players.Count) return;
        _players[index].Name = name;
        Changed?.Invoke();
    }

    public void AddScore(int index, int delta)
    {
        if (index < 0 || index >= _players.Count) return;
        _players[index].Score += delta;
        Changed?.Invoke();
    }

    public void ResetScores()
    {
        foreach (var p in _players) p.Score = 0;
        Changed?.Invoke();
    }

    /// <summary>
    /// Sorteia um item do pool evitando repetir o último mostrado daquele pool.
    /// Retorna default se a lista estiver vazia.
    /// </summary>
    public T? PickRandom<T>(string poolKey, IReadOnlyList<T> items, Func<T, Guid> idOf)
    {
        if (items.Count == 0) return default;

        var pool = items;
        if (items.Count > 1 && _lastByPool.TryGetValue(poolKey, out var lastId))
        {
            var filtered = items.Where(i => idOf(i) != lastId).ToList();
            if (filtered.Count > 0) pool = filtered;
        }

        var chosen = pool[Random.Shared.Next(pool.Count)];
        _lastByPool[poolKey] = idOf(chosen);
        return chosen;
    }
}
