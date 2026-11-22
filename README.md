# Intergalactic Football

7v7 football as a turn-based tactics game on a 15×9 grid. Each team is a species with its own powers, and each team gets 3 actions per turn. See the [core concept](docs/concept.md) for the pitch and [rules v0.1](docs/rules.md) for exactly what the engine does.

This repository holds the first build's **rules engine**, a plain C# assembly with no UnityEngine references. It contains:

- the match state
- the action resolver
- seeded rolls
- Gravions and Blinkers with their six powers and two ultimates
- an AI opponent
- a terminal harness for playing matches and running AI-vs-AI pacing sims

The Unity client hasn't been started. The engine is packaged so a Unity 6 project can reference it directly.

## Layout

```
packages/com.intergalacticfootball.rules/   Unity package (UPM), the single source of truth
  Runtime/                                  rules engine (asmdef with noEngineReferences: true)
    Core/      Tile, Pitch, Stats, RulesConfig (every tunable number), PCG32 rolls
    Content/   Species, powers, formations (first-build data; ScriptableObjects later)
    Match/     MatchState, setup, Scenario (puzzle and test positions)
    Actions/   MatchEngine (Validate / Preview / Apply), contests, pathing, legal-action generator
    Ai/        AiPlayer (expected-value search over the 3 actions), Evaluator
  Tests/Editor/                             NUnit edit-mode tests (run in Unity and with dotnet test)
dotnet/
  Rules/        netstandard2.1 project that compiles Runtime/ (C# 9, what Unity 6 supports)
  Rules.Tests/  net8.0 test project that compiles Tests/Editor/
  Cli/          terminal harness: play, hotseat, watch, sim
docs/           concept and rules
```

The `.csproj` files sit outside the package and build into `/artifacts`, so Unity never sees build output.

## Build and test

Needs the .NET 8 SDK.

```bash
dotnet test dotnet/Rules.Tests
```

## Play in the terminal

Play against the AI (you are Gravions; type `help` in game):

```bash
dotnet run --project dotnet/Cli -- play --ai medium
```

Pass-and-play:

```bash
dotnet run --project dotnet/Cli -- hotseat --home blinkers --away gravions
```

Watch the AI play itself:

```bash
dotnet run --project dotnet/Cli -- watch --seed 3
```

Commands use the letters on the board and `x,y` tiles: `m S 9,4` moves, `p Q` passes, `s top` shoots, `t A` tackles, `pw R 10,2` uses a power, `ult` fires the ultimate, `e` ends the turn and `h` asks the AI for a hint. Before any risky action the harness shows each contest's odds and asks you to confirm.

## Pacing and balance sims

The concept names pacing as the main risk. The sim plays AI against AI and reports goals per match, how many turns have a shot, actions used per turn, AI think time, and per-species shot, pass, dribble, tackle and power numbers:

```bash
dotnet run -c Release --project dotnet/Cli -- sim --matches 200
```

Over 200 matches with the medium AI on both sides, the current numbers give:

- about 4 goals per match
- a shot in about a third of turns
- shot conversion of 27–35%
- about 3 ms of AI time per action
- Gravions 77, draws 65, Blinkers 58

See [rules](docs/rules.md#what-the-sims-say) for the detail and the known gaps.

## Using the engine

```csharp
using IntergalacticFootball.Rules;
using IntergalacticFootball.Rules.Ai;

var setup = new MatchSetup(
    new TeamSetup(Catalog.Gravions, Formation.TwoThreeOne),
    new TeamSetup(Catalog.Blinkers, Formation.ThreeTwoOne),
    seed: 1234);
var match = MatchState.Create(setup);

// UI: what can the selected player do, and what are the odds?
var striker = match.GetPlayer(6);                              // ids: Home 0–6, Away 7–13 (K A B P Q R S)
var plans = Pathing.Reachable(match, striker);                 // move targets with dribble odds
var pass = GameAction.Pass(striker.Id, match.GetPlayer(3).Pos); // to the playmaker
var preview = MatchEngine.Preview(match, pass);                // preview.Error, .Contests, .SuccessPercent

// Commit: returns ordered events to animate (Moved, Contest, Intercepted, Goal, ...)
var result = MatchEngine.Apply(match, pass);

// AI turn
var ai = new AiPlayer(AiDifficulty.Medium);
while (!match.IsOver && match.ActiveSide == Side.Away)
    MatchEngine.Apply(match, ai.ChooseAction(match));
```

A replay is the `MatchSetup` (including the seed) plus the list of `GameAction`s. Applying them again reproduces the match roll for roll. That property is what lets a server validate async-PvP turns with this same assembly.

## Adding the engine to a Unity project

Create the Unity 6.3 LTS project in `unity/`; the `.gitignore` already covers its generated folders. Then reference the package from `unity/Packages/manifest.json`, and add the package to `testables` so its tests show up in the Test Runner:

```json
{
  "dependencies": {
    "com.intergalacticfootball.rules": "file:../../packages/com.intergalacticfootball.rules"
  },
  "testables": ["com.intergalacticfootball.rules"]
}
```

Unity writes `.meta` files into the package folder the first time it imports it; commit them. Keep `Runtime/` free of `UnityEngine`. The asmdef enforces that with `noEngineReferences`, and `EngineFreeTests` checks it.
