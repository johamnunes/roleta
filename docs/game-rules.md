# Regras do Show — "ROLETA"

> **Rascunho v1** — adaptação livre de *Rulette* (Game Changer / Dropout, S7E7) para um show
> de improviso AO VIVO. Este documento é a **fonte da verdade das mecânicas**: o software
> implementa o que está aqui. Tudo é editável — ajuste temas, números e exemplos à vontade.

---

## 1. Objetivo do jogo e como se vence

**3 participantes** improvisam ao vivo enquanto a Roleta vai jogando "regras" em cima deles.
Cada giro empilha mais uma restrição ou desafio. **O objetivo é sobreviver ao caos**: seguir
as regras que grudaram em você, pegar os outros quebrando as deles, e acumular pontos.

- **Vence** quem tiver mais **Pontos de Sobrevivência** no fim do jogo.
- **Ganha ponto** ao delatar um adversário que quebrou uma regra (antes do apresentador apitar)
  e ao executar bem uma Ação/Prompt.
- **Perde ponto** ao quebrar, e ser pego, uma regra grudada em você.
- Empate → **Giro de Desempate** (ver §9).

O tom é *comédia sob pressão*: quanto mais regras grudam, mais difícil falar uma frase normal
sem violar algo. É isso que diverte a plateia.

---

## 2. Componentes

| Componente | Papel |
|---|---|
| **Roleta física (12 seções, 4 cores)** | Fonte de aleatoriedade visível à plateia. Decide só a **cor** (qual pool). O conteúdo exato sai do app. |
| **App** | **Fonte da verdade** do tipo atual de cada seção. Sorteia o conteúdo, mostra na TV, controla pontuação e a evolução da roleta. |
| **3 Jogadores** | Improvisam, carregam regras grudadas, delatam uns aos outros. |
| **Apresentador-Juiz** | Opera o app/Stream Deck, narra, e é a **autoridade final** sobre quebra de regra e interpretação criativa. |
| **Stream Deck** | Botões físicos para disparar ações sem mexer no mouse. |
| **TV / Telão (Tela Pública)** | Mostra só conteúdo de show: **placar dos 3 participantes** no topo + **quadro grande** com o item revelado (regra/modificador/ação). |

### As 12 seções e as 4 cores
A roleta tem **12 seções** distribuídas entre **4 cores** (pools temáticos): Azul, Vermelho,
Verde e Branca. No início, todas as 12 seções valem **REGRA**. Ao longo do jogo elas vão virando
modificadores/ações (§6).

> ⚠️ A distribuição exata das 12 seções entre as 4 cores (quantas seções de cada) ainda será
> definida.

---

## 3. O que cada cor representa

Quatro temas que cobrem os eixos do improviso (voz, corpo, relação) e a participação do público:

- 🔵 **AZUL — "BOCA"** (regras de fala / verbais): restringem **como você fala** — vocabulário,
  sotaque, forma de frase. Audíveis, fáceis de pegar.
- 🔴 **VERMELHO — "CORPO"** (regras físicas / corporais): restringem **como você se move** —
  postura, gestos, contato com o cenário. Visíveis para a plateia inteira.
- 🟢 **VERDE — "GENTE"** (regras de interação / relação): restringem **como você se relaciona**
  com os outros dois — quem pode olhar, quem concorda com quem, status. Geram as melhores
  delações.
- ⚪ **BRANCA — "PLATEIA"** (regra construída pelo público): uma **frase pré-escrita com uma
  lacuna** (`______`) que a **plateia completa ao vivo** (ex.: "Você só pode falar sobre
  ______"). O público grita/escolhe o que entra na lacuna e a regra resultante gruda no jogador
  como qualquer outra. Coloca a plateia dentro do jogo.

---

## 4. Tipos de conteúdo

Cada seção, num dado momento, é de **um** destes três tipos:

1. **REGRA** — restrição persistente que **gruda no jogador** que girou. Vale até ser removida
   (por modificador) ou até o fim. Você **acumula** várias. Quebrou e foi pego = perde ponto.
   *Ex.: "Você só pode falar no diminutivo."*
2. **MODIFICADOR** — efeito **único e instantâneo** que mexe nas regras já em jogo (troca,
   clona, inverte…). Não gruda; resolve na hora e some. *Ex.: Troca — uma regra sua ↔ uma dele.*
3. **AÇÃO / PROMPT** — **desafio imediato** que o jogador executa agora, valendo ponto. Não
   gruda; é uma tarefa pontual. *Ex.: "Faça um comercial vendendo este objeto inútil."*

> Regra de ouro: **REGRA = permanente e acumula; MODIFICADOR e AÇÃO = pontuais e somem.** É o
> que faz o jogo evoluir de ordem para caos.

---

## 5. Fluxo de um GIRO e de uma RODADA

### Um GIRO (1 jogador)
1. É a vez do jogador X. Ele gira a **roleta física**.
2. A roleta para numa seção → define a **COR**. O app sabe o **tipo atual** daquela seção.
3. O apresentador aperta **REVELAR** (app/Stream Deck).
4. Conforme o tipo atual da seção:
   - **REGRA** → app sorteia 1 regra do pool da cor (sem repetir) e **gruda no jogador X**;
     mostra na TV.
   - **MODIFICADOR** → app sorteia 1 modificador; mostra na TV; X resolve o efeito na hora.
   - **AÇÃO** → app sorteia 1 ação; mostra na TV; X executa agora; o juiz valida (±1).
5. X passa a improvisar respeitando **todas** as regras grudadas nele. O jogo segue.

### Uma RODADA (3 giros)
- **1 rodada = os 3 jogadores giraram 1× cada** (ordem fixa: J1 → J2 → J3).
- **Ao fim de cada rodada completa**, o app executa a **Evolução da Roleta** (§6).
- A **delação** (§7) acontece **a qualquer momento**, inclusive durante a improvisação livre.

---

## 6. Evolução da Roleta (mecânica central, determinística)

A cada rodada o caos sobe: **1 das 12 seções deixa de ser REGRA**. Regra clara e determinística
que o app aplica (zero ambiguidade no palco):

**Disposição das seções** (índice fixo, cores intercaladas):
`1=🔵 2=🔴 3=🟢 4=🔵 5=🔴 6=🟢 7=🔵 8=🔴 9=🟢 10=🔵 11=🔴 12=🟢`

**Ordem de conversão:** da seção **12 para a 1** (de trás pra frente) — assim cada cor perde
regras de forma equilibrada (🟢12, 🔴11, 🔵10, 🟢9…).

**Em quê converte (alternância fixa):** 1ª → **MODIFICADOR**, 2ª → **AÇÃO**, 3ª →
**MODIFICADOR**, 4ª → **AÇÃO**… (alterna M, A, M, A…).

Isso garante: (a) previsível para app e apresentador; (b) o nº de seções "regra" cai exatamente
1 por rodada; (c) modificadores e ações entram na mesma proporção. O app tem um **override
manual** (forçar uma conversão específica para o ritmo do show), mas o default é automático.

> Exemplo de arco (N=6): R1 = 12 regras / 0 caos · R2 = 11/1 · R3 = 10/2 · R4 = 9/3 · R5 = 8/4 ·
> R6 = 7/5 → fim. Quanto mais avança, maior a chance de sair modificador ou ação.

*(Configurável: ordem de conversão e alternância podem ser ajustadas no setup.)*

> ⚠️ A disposição/intercalação acima foi escrita para 3 cores. Com a 4ª cor (Branca) ela será
> revisada junto com a distribuição das 12 seções (ver §2).

---

## 7. Pontuação, Delação (backstab) e o Juiz

### Como GANHA ponto
- **Delação correta** (apita um adversário que quebrou uma regra dele, antes do juiz notar):
  **+1 ao delator**, e o infrator **recebe uma regra extra** (nova regra sorteada da cor da
  regra violada gruda nele — adaptação direta do "passa uma regra ao infrator" do Rulette).
- **Ação/Prompt bem executada:** **+1** (julgamento do juiz).
- **Interpretação criativa premiada:** **+1 bônus** (ex.: cumprir duas regras conflitantes de
  um jeito esperto).

### Como PERDE ponto
- **Quebra pega** (pelo juiz OU por delação válida): **−1**.
- **Delação falsa** (apitou e a pessoa NÃO quebrou nada): **−1 ao delator** (anti-spam).

### A mecânica da Delação
- Qualquer jogador grita o bordão (ex.: **"REGRA!"**) apontando o infrator.
- O juiz decide **na hora**: procede → +1 delator, −1 infrator, +1 regra extra no infrator.
  Não procede → −1 delator.
- **Corrida contra o juiz:** se o apresentador apita a quebra **antes** de qualquer delação,
  não há transferência — só o −1 do infrator (ninguém lucra). Isso incentiva os jogadores a se
  vigiarem em vez de esperar o juiz.

### Papel do Apresentador-Juiz
- Autoridade final sobre **quebrou / não quebrou** e sobre **interpretação criativa**.
- **Regras conflitantes:** a mais recente prevalece, salvo decisão criativa do juiz.
- Controla o ritmo: pode segurar a próxima rodada para deixar uma cena render.

---

## 8. Modificadores

Efeitos pontuais. Os **5 canônicos** do show:

| Modificador | Efeito |
|---|---|
| **Inversão** | Toda regra tem um **oposto no verso**. **Vire uma regra no seu corpo** para o oposto (ex.: "fale baixinho" ↔ "fale gritando"). |
| **Troca** | **Troca mútua**: escolha uma regra sua e uma de outro participante — elas se trocam. |
| **CTRL+C CTRL+V** | Escolha uma regra **do seu corpo** e **copie-a para outro participante** — o original continua com você. |
| **ESQUERDA** | **Cada** participante escolhe uma regra sua e a move para o jogador à **esquerda**. |
| **DIREITA** | **Cada** participante escolhe uma regra sua e a move para o jogador à **direita**. |

### Sorteio (sacola sem reposição)
O app trata os modificadores como uma **sacola com 2 cópias de cada** kind ativo. No início
todos têm a mesma probabilidade; a cada saída a ficha é consumida (a chance daquele kind cai).
Quando a sacola esvazia, **reenche e reembaralha**.

> O app registra o efeito de cada modificador no estado (quem perdeu/ganhou regra), pois isso
> muda o placar futuro e a TV precisa refletir as regras ativas de cada jogador.

---

## 9. Término e desempate

**Término padrão:** por rodadas — o jogo acaba ao fim da **Rodada N** (sugestão **N = 6**, ~20–30
min; configurável no setup).

**Gatilho opcional "Rodada Final do Caos":** quando **metade das seções (6) já virou caos**, o
juiz pode declarar uma última rodada onde toda quebra vale **−2** e toda delação vale **+2**
(clímax).

**Desempate:** cada empatado faz **1 Giro de Desempate** valendo uma Ação/Prompt; quem o juiz
julgar melhor leva o jogo.

---

## 10. Conteúdo de exemplo (pronto pra palco)

> Tudo em PT-BR, executável ao vivo, engraçado mas julgável. Cada regra tem um **oposto** para
> o FLIP.

### 🔵 AZUL — BOCA (regras de fala)
1. **Diminutivo eterno** — tudo no diminutivo ("vou pegar um copinho de aguinha"). *(oposto: aumentativo eterno)*
2. **Sotaque sorteado** — fale com o sotaque que o juiz escolher. *(oposto: sem nenhum sotaque, robótico)*
3. **Sem a letra "S"** — nenhuma palavra com "s". *(oposto: só palavras com "s")*
4. **Pergunta sempre** — toda fala sua é uma pergunta. *(oposto: nunca pergunte nada)*
5. **Rima obrigatória** — termine cada fala rimando com a fala anterior. *(oposto: proibido rimar)*
6. **Volume 11** — só pode falar gritando. *(oposto: só pode sussurrar)*
7. **Terceira pessoa** — fale de si na terceira pessoa ("o Carlos acha que…"). *(oposto: só "eu, eu, eu")*
8. **Locução esportiva** — narre tudo como narrador de futebol. *(oposto: tudo como sussurro de documentário)*

### 🔴 VERMELHO — CORPO (regras físicas)
1. **Joelhos dobrados** — sempre meio agachado. *(oposto: sempre na ponta dos pés)*
2. **Nunca pare** — sempre em movimento. *(oposto: estátua, só mexe a boca)*
3. **Uma mão só** — uma das mãos sempre escondida nas costas. *(oposto: gesticule com as duas o tempo todo)*
4. **Dança de fundo** — micro-passos de dança o tempo todo. *(oposto: rígido como um soldado)*
5. **Equilíbrio** — só apoiado num pé de cada vez. *(oposto: sempre com os dois pés colados)*
6. **Gigante** — interprete como se fosse 3 m mais alto. *(oposto: minúsculo, encolhido)*
7. **Ímã de objeto** — sempre tocando um objeto do cenário. *(oposto: não pode tocar em nada)*
8. **Câmera lenta** — todos os movimentos em câmera lenta. *(oposto: tudo acelerado, 2×)*

### 🟢 VERDE — GENTE (regras de interação)
1. **Concorde sempre** — concorde com tudo que o J1 disser. *(oposto: discorde de tudo do J1)*
2. **Olho no chão** — nunca olhe direto para o jogador à sua direita. *(oposto: encare sem piscar)*
3. **Bajulador** — elogie alguém em cada fala. *(oposto: critique alguém em cada fala)*
4. **Súdito** — trate um jogador como rei/rainha. *(oposto: trate-o como serviçal)*
5. **Eco** — repita a última palavra do outro antes de responder. *(oposto: nunca repita palavra de ninguém)*
6. **Sem "não"** — proibido negar diretamente (estilo "sim, e…"). *(oposto: comece toda fala com "não")*
7. **Ciúme** — fique com ciúme sempre que dois conversarem sem você. *(oposto: ignore totalmente os outros)*
8. **Tradutor** — só fale "traduzindo" o que outro jogador disse. *(oposto: contradiga o que o outro disse)*

### ⚪ BRANCA — PLATEIA (frase com lacuna que o público completa)
1. **Tema livre** — "A plateia escolhe: você só pode falar sobre ______."
2. **Palavra mágica** — "Toda fala sua tem que incluir a palavra ______ (a plateia grita)."
3. **Personagem** — "Você age como se fosse ______ (a plateia escolhe)."
4. **Medo** — "Você tem medo mortal de ______ (a plateia decide)."
5. **Movimento** — "Você só pode se mover como ______ (a plateia escolhe)."
6. **Obsessão** — "Sua obsessão do momento é ______ (a plateia define)."

> Regras brancas não têm "oposto" para o Flip (são preenchidas ao vivo pela plateia).

### ⚙️ MODIFICADORES
Inversão · Troca · CTRL+C CTRL+V · ESQUERDA · DIREITA *(detalhes na §8)*.

### 🎬 AÇÕES / PROMPTS
1. **Discurso relâmpago** — 15 s convencendo a plateia de que "segunda é o melhor dia".
2. **Cena muda** — interprete uma despedida de aeroporto sem falar, 20 s.
3. **Confissão** — confesse um "crime" absurdo e fictício, com lágrimas.
4. **Comercial** — venda um objeto inútil do cenário como revolucionário.
5. **Música improvisada** — cante 15 s sobre o que o último jogador falou.
6. **Estátua viva** — congele numa pose dramática até o próximo giro.
7. **Entrevista** — vire repórter e entreviste outro jogador por 20 s.
8. **Troca de papéis** — imite outro jogador (voz + corpo) por 20 s.

---
---

# Anexo — Modelo de Domínio (ponte para a implementação)

> Alto nível, sem código. Guia a estrutura de `Roleta.Domain` / `Roleta.Application`.
> Persistência em SQLite; RNG com `Random` injetável (seed) para testes.

## A. Entidades / estado

- **GameSession** (agregado raiz): `Id`, `StartedAt`, `EndedAt`, `State` (máquina, §C),
  `MaxRounds` (default 6), `CurrentRoundNumber`, `CurrentSpinIndexInRound` (0–2),
  `ActivePlayerId`, `ConversionCounter` (alimenta a próxima seção a converter e a alternância
  M/A), `ChaosFinalRoundActive`. Coleções: `Players[3]`, `WheelSections[12]`, `Rounds[]`,
  `ScoreEvents[]`, `AssignedRules[]`.
- **Player:** `Id`, `Name`, `TurnOrder` (1–3), `Score`. Derivado: regras ativas grudadas.
- **WheelSection** (12 instâncias, espelham a roleta física): `Id`, `Index` (1–12), `Color`,
  `CurrentType` (`Regra|Modificador|Acao`), `ConvertedAtRound?`. Invariante: cores intercaladas
  por índice (§6).
- **RuleDefinition** (catálogo por cor): `Id`, `Color`, `Text`, `OppositeText` (FLIP), notas p/
  o juiz.
- **ModifierDefinition:** `Id`, `Kind` (`Flip|Swap|Clone|Left|Right`), `Name` (texto da TV).
- **ActionDefinition:** `Id`, `Text`, `SuggestedDurationSeconds`, `ScoreOnSuccess` (default +1).
- **AssignedRule** (instância em jogo, mutável): `Id`, `RuleDefinitionId`, `OwnerPlayerId`,
  `IsFlipped`, `OriginSpinId`, `AssignedAtRound`, `IsActive` (false se Amnésia), `TemporaryUntil?`
  (Espelho).
- **Round:** `Id`, `Number`, `Spins[3]`, `CompletedAt`, `EvolutionApplied`.
- **Spin:** `Id`, `RoundId`, `PlayerId`, `LandedColor`, `ResolvedType`, `RevealedDefinitionId`,
  `Timestamp`.
- **ScoreEvent** (audit log imutável): `Id`, `Timestamp`, `PlayerId`, `Delta` (+1/−1/+2/−2),
  `Reason` (`RuleBrokenCaught|CalloutSuccess|CalloutFalse|ActionSuccess|CreativeBonus|ChaosFinal`),
  `RelatedPlayerId?`, `RelatedAssignedRuleId?`, `SpinId?`.
- **Pools em runtime:** RulePool (por cor), ModifierPool, ActionPool — IDs disponíveis vs. já
  sorteados, para **não-repetição**.

## B. Telas (projeções)
- **ControlView** (privada): estado completo das 12 seções, regras grudadas por jogador, placar,
  comandos, notas do juiz, próxima conversão prevista.
- **PublicView** (TV): só a revelação atual (com cor) + placar + badges de regras ativas por
  jogador. **Nada de controles nem pools.**

## C. Máquina de estados
`Setup → RoundStart → AwaitingSpin → Revealing → Resolving` (loop dos 3 giros) `→ RoundEnd →
EvolutionApplied →` (próxima rodada ou) `GameOver → Tiebreak? → GameOver`.
Evento transversal **`Callout`** (resolve delação, gera ScoreEvents, pode grudar nova regra no
infrator) não altera o estado principal.

## D. Comandos (app / Stream Deck)
`StartGame(players, maxRounds)` · `SetLandedColor(cor)` · `Reveal()` ·
`AssignRuleToActivePlayer()` · `ResolveModifier(target?, rule?)` · `JudgeAction(success, bonus?)` ·
`Callout(accuserId, accusedId)` + `JudgeCallout(valid)` · `MarkRuleBroken(playerId, ruleId)` ·
`NextSpin()` / `EndRound()` · `ApplyEvolution()` (override manual) · `AdjustScore(playerId, delta,
reason)` · `ToggleDisplay(show/hide)` · `Undo()` · `EndGame()` (mostra o `EndingScript` do jogo
carregado no controle **sem** limpar placar/pools — o jogo continua carregado).

## E. Sorteio e não-repetição
- **Regra da cor** (Revealing/Rule): sortear do `RulePool[cor]` **sem repetir** na sessão
  (embaralhar IDs restantes com `Random.Shuffle` e consumir do topo, ou sortear índice e
  remover). Pool esgotado → avisar o juiz (ou reembaralhar, conforme política).
- **Modificador:** sacola com **2 cópias de cada** kind ativo; consome sem reposição; sacola
  vazia → reenche e reembaralha (§8).
- **Ação:** sem repetição até esgotar, depois reembaralha.
- **Regra extra ao infrator** (delação válida): do `RulePool[cor da regra violada]`; se esgotou,
  qualquer cor.
- Use `Random` **injetável** (seed) para testes determinísticos. Não use RNG criptográfico
  (overkill para um jogo).

## F. Invariantes a garantir
- Sempre 12 `WheelSection`; nº de seções `Regra` = `12 − ConversionCounter`.
- `ConversionCounter` cresce no máximo 1 por rodada.
- Soma do placar = soma dos `Delta` de todos os `ScoreEvent` (auditável via log → habilita Undo).
- Toda mudança de regra grudada (Troca/Clone/Inversão/Esquerda/Direita) registrada para a
  PublicView refletir e o Undo funcionar.
- **Persistir no SQLite antes de notificar a UI** → resume após crash sem perda.

---

## Pontos em aberto (decidir conforme iteramos)
- Confirmar **N de rodadas** e se a "Rodada Final do Caos" entra no v1.
- Bordão oficial da delação (ex.: "REGRA!").
- Mapa físico do Stream Deck (quais botões = quais comandos da §D).
- Tema/nomes definitivos das 3 cores e revisão do conteúdo de exemplo (§10).
