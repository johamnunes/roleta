# AGENTS.md — Roleta

> Instruções para agentes de IA (Claude Code, Cursor, Copilot, etc.).
> Mantenha curto (< 150 linhas) e acionável. **NÃO** repita aqui regras já garantidas por
> `.editorconfig` / `dotnet format` / CI.

## 1. Visão geral

**"Roleta"** é um software de palco para um **show de improviso AO VIVO** (adaptação livre do
jogo *Rulette*, do Game Changer / Dropout). O apresentador opera o app num notebook; o público
acompanha por uma TV/telão. Há **duas telas lógicas**:

- **CONTROLE** (`/controle`, privada) — todos os controles, vista só pelo apresentador.
- **PÚBLICO** (`/publico`, TV) — só conteúdo de show: placar dos 3 participantes + quadro
  grande com o item revelado. **Nenhum controle.**

Atalhos físicos rápidos vêm de um **Elgato Stream Deck**.

⚠️ **Prioridade nº 1 é confiabilidade ao vivo.** **Tudo roda LOCAL e OFFLINE.** Nada pode
depender de internet, rede externa ou serviço em nuvem.

A **fonte da verdade das regras do jogo** é [`docs/game-rules.md`](docs/game-rules.md). Em
conflito: as regras do jogo vencem para mecânica; este arquivo vence para convenções de código.

## 2. Stack

- **Linguagem:** C# / **.NET 10 (LTS)**. `<Nullable>enable</Nullable>`,
  `<ImplicitUsings>enable</ImplicitUsings>`, `<LangVersion>latest</LangVersion>`.
- **UI:** **Blazor Web App (Interactive SSR)** — um único processo ASP.NET Core serve
  `/controle` e `/publico`. Sincronização das duas telas em tempo real via **circuito SignalR**
  (nativo do Blazor Server) — sem polling, sem JS custom.
- **Stream Deck:** ação **"Web Request" / HTTP POST** para Minimal API em `localhost` (sem
  plugin custom). **Toda ação do Stream Deck também existe na tela de Controle** (e vice-versa).
- **Banco:** **SQLite via EF Core** (`Microsoft.EntityFrameworkCore.Sqlite`), local, modo WAL.
- **Testes:** xUnit (projeto `*.Tests`).

🍎 **Cross-platform — o show roda em macOS (MacBook Air M2, 8GB, arm64).** Pode-se desenvolver
no Windows, mas o alvo é o Mac. **Não use nenhuma API Windows-only.** Publicação alvo:
`dotnet publish -r osx-arm64`.

## 3. Comandos essenciais

```bash
dotnet restore                              # restaura deps (offline após a 1ª vez)
dotnet build                                # compila a solução
dotnet run --project src/Roleta.Web         # roda o app (http://localhost:5005)
dotnet watch --project src/Roleta.Web run   # dev com hot reload
dotnet test                                 # roda os testes
dotnet format                               # formata/checa estilo
dotnet ef migrations add <Nome> --project src/Roleta.Infrastructure   # nova migration
dotnet publish src/Roleta.Web -c Release -r osx-arm64 --self-contained # build p/ o Mac do show
```

**Sempre** rode `dotnet build` **e** `dotnet test` (ou `dotnet format --verify-no-changes`)
verdes antes de declarar uma tarefa concluída.

## 4. Estrutura de pastas

```
Roleta.slnx
src/
  Roleta.Domain/          # tipos e invariantes do jogo. Sem dependências de infra.
  Roleta.Application/      # GameEngine (Singleton): ÚNICO lugar que muda o estado. Comandos.
  Roleta.Infrastructure/  # EF Core + SQLite (IGameStore, migrations).
  Roleta.Web/             # Blazor Web App + Minimal API p/ Stream Deck. O executável.
data/   roleta.db         # SQLite (fora de bin/). Não comitar.
tests/  Roleta.Application.Tests/   # cobre as mecânicas de jogo.
docs/   game-rules.md     # FONTE DA VERDADE das regras.
```

## 5. Convenções de código

Formatação é garantida por `.editorconfig` + `dotnet format`. O que o agente deve seguir:

- **Nullable habilitado**; evite `!` (null-forgiving). Trate nulos explicitamente.
- **Async:** sufixo `Async`, propague `CancellationToken`. **Nunca** `async void` (exceto
  handlers de evento). Sem `.Result` / `.Wait()` (deadlock).
- **Naming:** `PascalCase` (tipos/métodos/props), `camelCase` (locais/params),
  `_camelCase` (campos privados), `IFoo` (interfaces).
- **Estado de jogo imutável:** prefira `record`/tipos imutáveis; um `enum` por conceito
  (`SectionType { Rule, Modifier, Action }`, `Color { Blue, Red, Green }`).
- **Idioma:** **identificadores (classes, propriedades, enums, métodos) sempre em inglês.**
  Apenas o **texto persistido** (conteúdo das regras/ações) e a **UI/mensagens ao usuário** em
  PT-BR. Comentários podem ser em PT-BR.

### Glossário do domínio (use estes termos)

| Termo (PT)        | Código     | Significado |
|-------------------|------------|-------------|
| Giro / Spin       | `Spin`     | Um acionamento da roleta para 1 participante |
| Rodada / Round    | `Round`    | 3 giros (os 3 participantes giraram 1× cada) |
| Seção             | `Section`  | 1 das 12 posições da roleta física |
| Cor               | `Color`    | Grupo/pool: Blue, Red, Green |
| Regra             | `Rule`     | Restrição permanente que "gruda" no jogador |
| Modificador       | `Modifier` | Efeito pontual (`ModifierKind`: Flip/Inversão, Swap/Troca, Clone/Ctrl+C+V, Left/Esquerda, Right/Direita) |
| Ação / Prompt     | `Action`   | Desafio imediato sorteado, valendo ponto |
| Delação / Backstab| `CallOut`  | Acusar quebra de regra antes do apresentador notar |

## 6. Regras de desenvolvimento (workflow)

- **Passos pequenos e verificáveis.** Incrementos que compilam e passam nos testes. Não faça
  refactors grandes sem pedir.
- **Lógica de jogo no `Domain`/`Application`, testável, separada da UI.** Mecânicas (sorteio,
  pontuação, estado das 12 seções, evolução regra→modificador/ação) ficam aqui e têm testes.
  **O app é a fonte da verdade do tipo atual de cada uma das 12 seções.**
- **Estado fora do circuito:** o `GameState` vive num **singleton em memória**, espelhado no
  SQLite **a cada mutação (persistir ANTES de notificar a UI)**. Assim uma reconexão de
  navegador ou um crash nunca perde a partida → "Continuar de onde parou".
- **RNG testável:** sorteios aceitam um `Random`/seed injetável. Não-repetição via shuffle dos
  IDs restantes do pool (ver `docs/game-rules.md`).
- **Offline-first:** **não** adicione dependências que exijam internet/serviço externo. Prefira
  a BCL e poucos pacotes leves. Dependência nova precisa de justificativa.
- **Paridade Stream Deck ↔ Controle:** toda ação acionável pelo Stream Deck também existe na
  tela de Controle, e ambas chamam **os mesmos comandos** do `GameEngine`.
- **Confiabilidade ao vivo:** trate erros sem derrubar o app; ações idempotentes/debounced
  quando possível; nada que possa travar a UI durante o show; sempre haja **Desfazer**.

## 7. Tela do PÚBLICO — não vazar spoiler

`/publico` recebe apenas um **PublicView projetado** com o que pode ser mostrado (item
revelado + placar + regras ativas por jogador). Os controles **não são renderizados** nessa
rota (não é "esconder com CSS"). **Nunca** mande ao público: pools não sorteados, controles,
estado interno, próximas seções ou qualquer coisa que estrague a surpresa.

## 8. NÃO faça

- ❌ Dependências de rede/cloud, telemetria online ou auth externa.
- ❌ APIs Windows-only (o show roda em macOS arm64).
- ❌ Lógica de jogo dentro de componentes/code-behind da UI.
- ❌ Revelar conteúdo não sorteado ou controles na tela PÚBLICO.
- ❌ `async void`, `.Result`/`.Wait()`, ignorar `CancellationToken`.
- ❌ Inventar regras do jogo — consulte [`docs/game-rules.md`](docs/game-rules.md); em dúvida, pergunte.
- ❌ Concluir tarefa sem `dotnet build` + `dotnet test` verdes.
- ❌ Comitar `data/*.db`, `bin/`, `obj/` ou segredos.

> **Cursor:** este `AGENTS.md` na raiz já é lido nativamente por Cursor e Claude Code. Se
> quiser regras com auto-attach por pasta, use `.cursor/rules/*.mdc` — sem duplicar conteúdo.
