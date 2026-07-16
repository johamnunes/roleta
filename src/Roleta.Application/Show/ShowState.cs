using Roleta.Domain.Catalog;
using Roleta.Domain.Enums;
using Roleta.Domain.Games;

namespace Roleta.Application.Show;

public enum RevealKind { Rule, Modifier, Action }

/// <summary>O que está atualmente no quadro grande do telão.</summary>
public sealed record Reveal(
    RevealKind Kind,
    string Title,
    string Text,
    Color? Color,
    ModifierKind? ModifierKind = null);

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
    private readonly Random _rng;

    // ---- Cronômetro (contagem regressiva da ação) ----
    private readonly System.Threading.Timer _ticker;
    private readonly object _timerLock = new();

    public int TimerTotalSeconds { get; private set; }
    public int TimerRemainingSeconds { get; private set; }
    public bool TimerRunning { get; private set; }

    public ShowState(Random? rng = null)
    {
        _rng = rng ?? Random.Shared;
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
        IsAudiencePending = false;
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

    /// <summary>
    /// Teaser da regra branca: telão mostra "Regra da Plateia" + confete,
    /// e o controle espera o apresentador clicar em Revelar.
    /// </summary>
    public bool IsAudiencePending { get; private set; }

    private readonly List<RuleDefinition> _ruleHistory = [];
    private readonly HashSet<Guid> _flippedRuleIds = [];

    /// <summary>Regras que já saíram nesta sessão (mais recente primeiro).</summary>
    public IReadOnlyList<RuleDefinition> RuleHistory => _ruleHistory;

    /// <summary>True se a regra está no estado invertido (oposto) nesta sessão.</summary>
    public bool IsRuleFlipped(Guid ruleId) => _flippedRuleIds.Contains(ruleId);

    // ---- Jogo carregado ----
    private readonly List<RuleDefinition> _gameRules = [];
    private readonly List<ModifierDefinition> _gameModifiers = [];
    private readonly List<ActionDefinition> _gameActions = [];
    private readonly HashSet<Guid> _usedRuleIds = [];
    private readonly HashSet<Guid> _usedActionIds = [];

    // ---- Sacola de modificadores (cópias ×2, sem reposição) ----
    private readonly List<ModifierDefinition> _modifierPool = [];
    private readonly List<Guid> _modifierBag = [];

    /// <summary>Fichas restantes na sacola atual (para testes / UI).</summary>
    public int ModifierBagRemaining => _modifierBag.Count;

    /// <summary>Nome do jogo carregado, ou null se está em modo livre (sorteia do catálogo inteiro).</summary>
    public string? ActiveGameName { get; private set; }

    /// <summary>Script de encerramento do jogo carregado (lido no botão Fim de jogo).</summary>
    public string? ActiveEndingScript { get; private set; }

    /// <summary>Pontos base do jogo carregado (botão Atribuir pontos base no controle).</summary>
    public int ActiveBasePoints { get; private set; }

    public bool HasActiveGame => ActiveGameName is not null;

    public int RemainingRules(Color color) =>
        _gameRules.Count(r => r.Color == color && !_usedRuleIds.Contains(r.Id));
    public int RemainingActions => _gameActions.Count(a => !_usedActionIds.Contains(a.Id));
    public int RemainingModifiers => _modifierPool.Count;

    /// <summary>Disparado a cada mutação do estado.</summary>
    public event Action? Changed;

    public void SetReveal(Reveal reveal)
    {
        Current = reveal;
        DisplayVisible = true;
        IsFlipReveal = false;
        CurrentRule = null;
        IsShowingOpposite = false;
        IsAudiencePending = false;
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
        IsAudiencePending = false;
        RevealSeq++;

        // histórico: mais recente no topo, sem duplicar
        _ruleHistory.RemoveAll(r => r.Id == rule.Id);
        _ruleHistory.Insert(0, rule);

        Changed?.Invoke();
    }

    /// <summary>
    /// Mostra a regra com animação de ABRIR, no estado persistido (base ou invertida).
    /// Usado por revelar/Mostrar.
    /// </summary>
    public void ShowRule(RuleDefinition rule) =>
        RevealRuleInternal(rule, opposite: IsRuleFlipped(rule.Id), flipAnimation: false);

    /// <summary>
    /// Arma a regra branca: telão entra no teaser da plateia; o texto só sai após
    /// <see cref="RevealAudienceRule"/>.
    /// </summary>
    public void ArmAudienceRule(RuleDefinition rule)
    {
        Current = null;
        DisplayVisible = true;
        IsFlipReveal = false;
        IsShowingOpposite = false;
        CurrentRule = rule;
        IsAudiencePending = true;
        RevealSeq++;

        _ruleHistory.RemoveAll(r => r.Id == rule.Id);
        _ruleHistory.Insert(0, rule);

        Changed?.Invoke();
    }

    /// <summary>Revela no telão a regra branca armada pelo apresentador.</summary>
    public void RevealAudienceRule()
    {
        if (!IsAudiencePending || CurrentRule is null) return;
        ShowRule(CurrentRule);
    }

    /// <summary>Cancela o teaser da plateia sem mostrar o texto no telão.</summary>
    public void CancelAudiencePending()
    {
        if (!IsAudiencePending) return;
        IsAudiencePending = false;
        CurrentRule = null;
        Changed?.Invoke();
    }

    /// <summary>
    /// Alterna o estado persistido base/oposto da regra e mostra no telão com animação de virar.
    /// </summary>
    public void ToggleRule(RuleDefinition rule)
    {
        if (string.IsNullOrWhiteSpace(rule.OppositeText)) return;

        if (!_flippedRuleIds.Add(rule.Id))
            _flippedRuleIds.Remove(rule.Id);

        RevealRuleInternal(rule, opposite: IsRuleFlipped(rule.Id), flipAnimation: true);
    }

    /// <summary>
    /// Define o pool da sacola de modificadores. Se o conjunto de IDs mudou, reconstrói e
    /// reembaralha (cópias × <see cref="CanonicalModifiers.CopiesPerKind"/>).
    /// </summary>
    public void SetModifierPool(IEnumerable<ModifierDefinition> modifiers)
    {
        var active = modifiers.Where(m => m.IsActive).ToList();
        var newIds = active.Select(m => m.Id).OrderBy(id => id).ToList();
        var currentIds = _modifierPool.Select(m => m.Id).OrderBy(id => id).ToList();
        if (newIds.SequenceEqual(currentIds))
        {
            // Atualiza nomes/kinds em memória sem resetar a sacola
            _modifierPool.Clear();
            _modifierPool.AddRange(active);
            return;
        }

        _modifierPool.Clear();
        _modifierPool.AddRange(active);
        RefillModifierBag();
    }

    /// <summary>Quantas fichas do kind ainda restam na sacola atual.</summary>
    public int ModifierBagCount(ModifierKind kind) =>
        _modifierBag.Count(id => _modifierPool.Any(m => m.Id == id && m.Kind == kind));

    private void RefillModifierBag()
    {
        _modifierBag.Clear();
        foreach (var m in _modifierPool)
        {
            for (var i = 0; i < CanonicalModifiers.CopiesPerKind; i++)
                _modifierBag.Add(m.Id);
        }

        for (var i = _modifierBag.Count - 1; i > 0; i--)
        {
            var j = _rng.Next(i + 1);
            (_modifierBag[i], _modifierBag[j]) = (_modifierBag[j], _modifierBag[i]);
        }
    }

    /// <summary>
    /// Carrega um jogo: aplica os nomes dos jogadores, define os pools de conteúdo,
    /// zera o placar, o histórico e os itens "já usados".
    /// </summary>
    public void LoadGame(Game game)
    {
        ActiveGameName = game.Name;
        ActiveEndingScript = string.IsNullOrWhiteSpace(game.EndingScript) ? null : game.EndingScript.Trim();
        ActiveBasePoints = Math.Clamp(game.BasePoints, 0, 99);

        _players[0].Name = string.IsNullOrWhiteSpace(game.Player1) ? "Jogador 1" : game.Player1;
        _players[1].Name = string.IsNullOrWhiteSpace(game.Player2) ? "Jogador 2" : game.Player2;
        _players[2].Name = string.IsNullOrWhiteSpace(game.Player3) ? "Jogador 3" : game.Player3;
        foreach (var p in _players) p.Score = 0;

        _gameRules.Clear(); _gameRules.AddRange(game.Rules);
        _gameModifiers.Clear(); _gameModifiers.AddRange(game.Modifiers);
        _gameActions.Clear(); _gameActions.AddRange(game.Actions);
        _usedRuleIds.Clear();
        _usedActionIds.Clear();

        // Novo jogo: sempre reconstrói a sacola (mesmo se os IDs forem iguais aos anteriores).
        _modifierPool.Clear();
        _modifierPool.AddRange(game.Modifiers.Where(m => m.IsActive));
        RefillModifierBag();

        _ruleHistory.Clear();
        _flippedRuleIds.Clear();
        Current = null;
        IsFlipReveal = false;
        CurrentRule = null;
        IsShowingOpposite = false;
        IsAudiencePending = false;
        Changed?.Invoke();
    }

    /// <summary>Volta ao modo livre (sorteia do catálogo inteiro, sem controle de repetição).</summary>
    public void ClearGame()
    {
        ActiveGameName = null;
        ActiveEndingScript = null;
        ActiveBasePoints = 0;
        _gameRules.Clear();
        _gameModifiers.Clear();
        _gameActions.Clear();
        _usedRuleIds.Clear();
        _usedActionIds.Clear();
        _modifierPool.Clear();
        _modifierBag.Clear();
        Changed?.Invoke();
    }

    /// <summary>Sorteia uma regra da cor no jogo carregado, sem repetir. false se acabaram.</summary>
    public bool DrawRule(Color color)
    {
        var candidates = _gameRules.Where(r => r.Color == color && !_usedRuleIds.Contains(r.Id)).ToList();
        if (candidates.Count == 0) return false;
        var rule = candidates[_rng.Next(candidates.Count)];
        _usedRuleIds.Add(rule.Id);
        if (color == Color.White)
            ArmAudienceRule(rule);
        else
            ShowRule(rule);
        return true;
    }

    /// <summary>Sorteia uma ação do jogo carregado, sem repetir. false se acabaram.</summary>
    public bool DrawAction()
    {
        var candidates = _gameActions.Where(a => !_usedActionIds.Contains(a.Id)).ToList();
        if (candidates.Count == 0) return false;
        var action = candidates[_rng.Next(candidates.Count)];
        _usedActionIds.Add(action.Id);
        SetReveal(new Reveal(RevealKind.Action, string.Empty, action.Text, null));
        return true;
    }

    /// <summary>
    /// Consome 1 ficha da sacola de modificadores (sem reposição). Sacola vazia → reenche.
    /// false se o pool está vazio.
    /// </summary>
    public bool DrawModifier()
    {
        if (_modifierPool.Count == 0) return false;
        if (_modifierBag.Count == 0) RefillModifierBag();

        var id = _modifierBag[0];
        _modifierBag.RemoveAt(0);
        var modifier = _modifierPool.First(m => m.Id == id);
        SetReveal(new Reveal(RevealKind.Modifier, string.Empty, modifier.Name, null, modifier.Kind));
        return true;
    }

    public void ClearReveal()
    {
        Current = null;
        IsFlipReveal = false;
        CurrentRule = null;
        IsShowingOpposite = false;
        IsAudiencePending = false;
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
    /// Define o placar de cada jogador para os pontos base do jogo carregado.
    /// No-op se não há jogo ou BasePoints ≤ 0.
    /// </summary>
    public bool AssignBaseScores()
    {
        if (!HasActiveGame || ActiveBasePoints <= 0) return false;
        foreach (var p in _players) p.Score = ActiveBasePoints;
        Changed?.Invoke();
        return true;
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

        var chosen = pool[_rng.Next(pool.Count)];
        _lastByPool[poolKey] = idOf(chosen);
        return chosen;
    }
}
