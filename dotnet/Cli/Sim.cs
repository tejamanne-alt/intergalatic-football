using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IntergalacticFootball.Rules;
using IntergalacticFootball.Rules.Ai;

namespace IntergalacticFootball.Cli;

/// <summary>
/// AI-vs-AI batch runs. The concept's main risk is pacing, so this reports how turns are spent and how long
/// the AI thinks, alongside balance numbers per species.
/// </summary>
static class Sim
{
    sealed class SpeciesStats
    {
        public int Matches, Wins, Draws, Goals, Conceded;
        public int Shots, Saves, Blocks, Misses;
        public int Passes, Completed, Intercepted, IntoSpace;
        public int Dribbles, DribblesWon, HeldOn;
        public int Tackles, TacklesWon;
        public int FluxGained, Ultimates;
        public readonly Dictionary<PowerId, int> Powers = new();
    }

    public static int Run(Options o)
    {
        var bySpecies = new Dictionary<string, SpeciesStats>();
        SpeciesStats For(SpeciesDef d) => bySpecies.TryGetValue(d.Id, out var st) ? st : bySpecies[d.Id] = new SpeciesStats();

        ulong firstSeed = o.Seed ?? 1;
        int totalGoals = 0, homeWins = 0, awayWins = 0, draws = 0;
        int firstWins = 0, secondWins = 0; // "first" = the --home species/AI, wherever it played
        int turns = 0, actionsUsed = 0, earlyEnds = 0, turnsWithShot = 0;
        long decisions = 0;
        double thinkMs = 0, maxThinkMs = 0;
        var scorelines = new Dictionary<string, int>();
        var clock = Stopwatch.StartNew();

        Console.WriteLine($"Simulating {o.Matches} matches: {o.Home.Name} ({o.HomeAi}) vs {o.Away.Name} ({o.AwayAi}){(o.Swap ? ", swapping sides every other match" : "")}.");
        Console.WriteLine("Species and AI level travel together when sides swap.");

        for (int m = 0; m < o.Matches; m++)
        {
            bool swapped = o.Swap && m % 2 == 1;
            var homeSpecies = swapped ? o.Away : o.Home;
            var awaySpecies = swapped ? o.Home : o.Away;
            var homeAi = new AiPlayer(swapped ? o.AwayAi : o.HomeAi, seed: 1000 + (ulong)m);
            var awayAi = new AiPlayer(swapped ? o.HomeAi : o.AwayAi, seed: 2000 + (ulong)m);
            var setup = new MatchSetup(
                new TeamSetup(homeSpecies, swapped ? o.AwayFormation : o.HomeFormation),
                new TeamSetup(awaySpecies, swapped ? o.HomeFormation : o.AwayFormation),
                firstSeed + (ulong)m);
            var s = MatchState.Create(setup);

            int actionsThisTurn = 0;
            bool shotThisTurn = false;
            while (!s.IsOver)
            {
                var side = s.ActiveSide;
                var team = For(s.Team(side).Species);
                var other = For(s.Team(side.Opponent()).Species);
                var ai = side == Side.Home ? homeAi : awayAi;

                var t0 = Stopwatch.GetTimestamp();
                var action = ai.ChooseAction(s);
                double ms = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
                thinkMs += ms;
                maxThinkMs = Math.Max(maxThinkMs, ms);
                decisions++;

                if (action.Type == ActionType.EndTurn && s.ActionsLeft > 0) earlyEnds++;
                if (action.Type != ActionType.EndTurn && action.Type != ActionType.Ultimate) actionsThisTurn++;
                if (action.Type == ActionType.Tackle) team.Tackles++;

                var result = MatchEngine.Apply(s, action);
                foreach (var e in result.Events)
                {
                    switch (e.Type)
                    {
                        case EventType.Shot: team.Shots++; shotThisTurn = true; break;
                        case EventType.Goal: team.Goals++; other.Conceded++; totalGoals++; break;
                        case EventType.Saved: team.Saves++; break;
                        case EventType.Blocked: team.Blocks++; break;
                        case EventType.Missed: team.Misses++; break;
                        case EventType.PassMade: team.Passes++; break;
                        case EventType.PassCompleted: team.Completed++; break;
                        case EventType.PassIntoSpace: team.IntoSpace++; break;
                        case EventType.Intercepted: team.Intercepted++; break;
                        case EventType.HeldOn: team.HeldOn++; break;
                        case EventType.TackleWon: if (action.Type == ActionType.Tackle) team.TacklesWon++; break;
                        case EventType.PowerUsed: team.Powers[e.Power] = team.Powers.GetValueOrDefault(e.Power) + 1; break;
                        case EventType.UltimateUsed: team.Ultimates++; break;
                        case EventType.FluxGained: For(s.Team(e.Side).Species).FluxGained += e.Amount; break;
                        case EventType.Contest when e.ContestKind == ContestKind.Dribble:
                            team.Dribbles++;
                            if (e.Success) team.DribblesWon++;
                            break;
                        case EventType.TurnEnded:
                            turns++;
                            actionsUsed += actionsThisTurn;
                            if (shotThisTurn) turnsWithShot++;
                            actionsThisTurn = 0;
                            shotThisTurn = false;
                            break;
                    }
                }
            }

            var home = s.Team(Side.Home);
            var away = s.Team(Side.Away);
            For(homeSpecies).Matches++;
            For(awaySpecies).Matches++;
            if (home.Score > away.Score) { homeWins++; For(homeSpecies).Wins++; if (swapped) secondWins++; else firstWins++; }
            else if (away.Score > home.Score) { awayWins++; For(awaySpecies).Wins++; if (swapped) firstWins++; else secondWins++; }
            else { draws++; For(homeSpecies).Draws++; For(awaySpecies).Draws++; }
            string line = $"{home.Score}-{away.Score}";
            scorelines[line] = scorelines.GetValueOrDefault(line) + 1;
            Console.WriteLine($"  #{m + 1,-3} {homeSpecies.Name,-9} {home.Score} - {away.Score} {awaySpecies.Name}");
        }

        int n = Math.Max(1, o.Matches);
        Console.WriteLine();
        Console.WriteLine("Pacing");
        Console.WriteLine($"  Goals per match        {totalGoals / (double)n:0.00}");
        Console.WriteLine($"  Actions used per turn  {actionsUsed / (double)Math.Max(1, turns):0.00} of 3 (turns ended early: {earlyEnds})");
        Console.WriteLine($"  Turns with a shot      {100.0 * turnsWithShot / Math.Max(1, turns):0}%");
        Console.WriteLine($"  AI think time          {thinkMs / Math.Max(1, decisions):0.0} ms avg, {maxThinkMs:0} ms max per action");
        Console.WriteLine($"  Results by side        home {homeWins}, draw {draws}, away {awayWins}");
        Console.WriteLine($"  Results by team        {o.Home.Name} ({o.HomeAi}) {firstWins}, draw {draws}, {o.Away.Name} ({o.AwayAi}) {secondWins}");
        Console.WriteLine($"  Common scores          {string.Join(", ", scorelines.OrderByDescending(kv => kv.Value).Take(5).Select(kv => $"{kv.Key} x{kv.Value}"))}");
        Console.WriteLine($"  Wall time              {clock.Elapsed.TotalSeconds:0.0} s");
        Console.WriteLine();

        foreach (var kv in bySpecies)
        {
            var st = kv.Value;
            var name = Catalog.SpeciesById(kv.Key).Name;
            int mm = Math.Max(1, st.Matches);
            Console.WriteLine($"{name}: {st.Wins}W {st.Draws}D {st.Matches - st.Wins - st.Draws}L, goals {st.Goals}-{st.Conceded}");
            Console.WriteLine($"  Shots {st.Shots / (double)mm:0.0}/match, scored {Pct(st.Goals, st.Shots)}, saved {Pct(st.Saves, st.Shots)}, blocked {Pct(st.Blocks, st.Shots)}, missed {Pct(st.Misses, st.Shots)}");
            Console.WriteLine($"  Passes {st.Passes / (double)mm:0.0}/match: completed {Pct(st.Completed, st.Passes)}, into space {Pct(st.IntoSpace, st.Passes)}, intercepted {Pct(st.Intercepted, st.Passes)}");
            Console.WriteLine($"  Dribble contests {st.Dribbles / (double)mm:0.0}/match, won {Pct(st.DribblesWon, st.Dribbles)}{(st.HeldOn > 0 ? $", held on {st.HeldOn}x" : "")}; tackles {st.Tackles / (double)mm:0.0}/match, won {Pct(st.TacklesWon, st.Tackles)}");
            string powers = st.Powers.Count == 0 ? "none" : string.Join(", ", st.Powers.OrderByDescending(p => p.Value).Select(p => $"{Catalog.Power(p.Key).Name} {p.Value / (double)mm:0.0}"));
            Console.WriteLine($"  Flux gained {st.FluxGained / (double)mm:0.0}/match; powers per match: {powers}; ultimates {st.Ultimates}");
        }
        return 0;
    }

    static string Pct(int a, int b) => b == 0 ? "-" : $"{100.0 * a / b:0}%";
}
