using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IntergalacticFootball.Rules;
using IntergalacticFootball.Rules.Ai;

namespace IntergalacticFootball.Cli;

/// <summary>Runs one match in the terminal with any mix of human and AI sides.</summary>
sealed class Game
{
    const string Help = """
          Commands (player labels are the letters on the board; coordinates are x,y)
            move <who> <x,y>        m S 9,4      Move along the safest path
            pass [who] <whom|x,y>   p Q          Pass to a teammate, or into space
            shoot [who] [row]       s top        Shoot; row = top|mid|bottom or 3|4|5 (default: best)
            tackle <who>            t A          Tackle the ball carrier from an adjacent tile
            power <who> [target]    pw R 10,2    Use a player's power (target: tile, teammate or row)
            ult [who whom]          ult P S      Team ultimate (needs full Flux, costs no action)
            end                     e            End the turn early
            info                    i            Squads, stats and powers
            hint                    h            What the AI would do here
            board                   b            Redraw the pitch
            quit                    q
        """;

    readonly Options o;
    readonly HashSet<Side> humans;
    readonly Dictionary<Side, AiPlayer> ais = new();
    MatchState s;

    public Game(Options options, IEnumerable<Side> humanSides)
    {
        o = options;
        humans = new HashSet<Side>(humanSides);
        ais[Side.Home] = new AiPlayer(o.HomeAi, seed: 11);
        ais[Side.Away] = new AiPlayer(o.AwayAi, seed: 23);
    }

    public int Run()
    {
        ulong seed = o.Seed ?? (ulong)DateTime.UtcNow.Ticks;
        var setup = new MatchSetup(new TeamSetup(o.Home, o.HomeFormation), new TeamSetup(o.Away, o.AwayFormation), seed);
        s = MatchState.Create(setup);

        Console.WriteLine($"Seed {seed}. Type 'help' for commands.");
        Console.WriteLine(Board.Squads(s));

        int lastDrawnTurn = -1;
        while (!s.IsOver)
        {
            bool human = humans.Contains(s.ActiveSide);
            // Humans see the board before every action; AI turns redraw once per turn.
            if (human || lastDrawnTurn != s.TurnIndex) Console.WriteLine(Board.Render(s, o.Color));
            lastDrawnTurn = s.TurnIndex;
            if (human)
            {
                if (!HumanAction()) return 0;
            }
            else
            {
                AiAction();
            }
        }

        Console.WriteLine(Board.Render(s, o.Color));
        var home = s.Team(Side.Home);
        var away = s.Team(Side.Away);
        string verdict = home.Score == away.Score ? "It's a draw." : $"{(home.Score > away.Score ? home : away).Species.Name} win.";
        Console.WriteLine($"  Full time: {home.Species.Name} {home.Score} - {away.Score} {away.Species.Name}. {verdict}");
        return 0;
    }

    void AiAction()
    {
        var action = ais[s.ActiveSide].ChooseAction(s);
        string who = s.Team(s.ActiveSide).Species.Name;
        var preview = MatchEngine.Preview(s, action);
        string odds = preview.Contests.Count > 0 ? $"  ({preview.SuccessPercent}%)" : "";
        Console.WriteLine($"> {who}: {Board.DescribeAction(s, action)}{odds}");
        var result = MatchEngine.Apply(s, action);
        PrintEvents(result);
        if (o.DelayMs > 0) Thread.Sleep(o.DelayMs);
    }

    /// <summary>Reads and applies one command. Returns false to quit.</summary>
    bool HumanAction()
    {
        while (true)
        {
            Console.Write($"{s.Team(s.ActiveSide).Species.Name} > ");
            var line = Console.ReadLine();
            if (line == null) return false;
            var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) continue;

            GameAction action;
            string error;
            switch (tokens[0].ToLowerInvariant())
            {
                case "q":
                case "quit":
                case "exit":
                    return false;
                case "?":
                case "help":
                    Console.WriteLine(Help);
                    continue;
                case "i":
                case "info":
                    Console.WriteLine(Board.Squads(s));
                    continue;
                case "b":
                case "board":
                    Console.WriteLine(Board.Render(s, o.Color));
                    continue;
                case "h":
                case "hint":
                    ShowHint();
                    continue;
                default:
                    (action, error) = Parse(tokens);
                    break;
            }

            if (error != null)
            {
                Console.WriteLine("  " + error);
                continue;
            }

            var preview = MatchEngine.Preview(s, action);
            if (!preview.IsLegal)
            {
                Console.WriteLine("  " + preview.Error);
                continue;
            }

            if (preview.Contests.Count > 0)
            {
                foreach (var c in preview.Contests)
                {
                    string versus = c.DefenderId >= 0 ? $" by {s.GetPlayer(c.DefenderId).Label}" : "";
                    Console.WriteLine($"  {c.Kind}{versus}: {c.Chance}%");
                }
                Console.WriteLine($"  Overall chance: {preview.SuccessPercent}%");
                if (o.Confirm)
                {
                    Console.Write("  Commit? [Y/n] ");
                    var answer = Console.ReadLine();
                    if (answer == null) return false;
                    if (answer.Trim().StartsWith("n", StringComparison.OrdinalIgnoreCase)) continue;
                }
            }

            PrintEvents(MatchEngine.Apply(s, action));
            return true;
        }
    }

    void ShowHint()
    {
        var ranked = new AiPlayer(AiDifficulty.Medium).Rank(s.Clone());
        Console.WriteLine("  The AI would consider:");
        foreach (var r in ranked.Take(3))
        {
            var p = MatchEngine.Preview(s, r.Action);
            string odds = p.Contests.Count > 0 ? $" ({p.SuccessPercent}%)" : "";
            Console.WriteLine($"    {Board.DescribeAction(s, r.Action)}{odds}");
        }
    }

    (GameAction, string) Parse(string[] t)
    {
        string cmd = t[0].ToLowerInvariant();
        var carrier = s.Carrier != null && s.Carrier.Side == s.ActiveSide ? s.Carrier : null;
        switch (cmd)
        {
            case "e":
            case "end":
                return (GameAction.EndTurn(), null);

            case "m":
            case "move":
            {
                if (t.Length < 3) return (null, "Usage: move <who> <x,y>");
                var p = Own(t[1]);
                if (p == null) return (null, $"No player '{t[1]}' on your team.");
                if (!Tile.TryParse(t[2], out var dest)) return (null, $"'{t[2]}' isn't a tile. Use x,y like 9,4.");
                var plan = Pathing.Find(s, p, dest);
                if (plan == null) return (null, $"{p.Label} can't reach {dest} this move (range {s.MoveRange(p)}).");
                return (GameAction.Move(p.Id, plan.Path), null);
            }

            case "p":
            case "pass":
            {
                Player p;
                string targetText;
                if (t.Length == 2)
                {
                    p = carrier;
                    targetText = t[1];
                }
                else if (t.Length >= 3)
                {
                    p = Own(t[1]);
                    targetText = t[2];
                }
                else return (null, "Usage: pass [who] <teammate|x,y>");
                if (p == null) return (null, "Your team doesn't have the ball.");
                var target = TargetTile(targetText);
                if (target == null) return (null, $"'{targetText}' isn't a teammate or a tile.");
                return (GameAction.Pass(p.Id, target.Value), null);
            }

            case "s":
            case "shoot":
            {
                var p = carrier;
                int? row = null;
                foreach (var arg in t.Skip(1))
                {
                    var who = Own(arg);
                    if (who != null) p = who;
                    else if (Row(arg) is int r) row = r;
                    else return (null, $"'{arg}' isn't a player or a goal row (top, mid, bottom).");
                }
                if (p == null) return (null, "Your team doesn't have the ball.");
                return (GameAction.Shoot(p.Id, row ?? BestRow(p, false)), null);
            }

            case "t":
            case "tackle":
            {
                Player p = t.Length >= 2 ? Own(t[1]) : s.PlayersOf(s.ActiveSide).FirstOrDefault(x => ActionGenerator.CanTackle(s, x));
                if (p == null) return (null, "Usage: tackle <who> (a player next to the ball carrier)");
                return (GameAction.Tackle(p.Id), null);
            }

            case "pw":
            case "power":
            {
                if (t.Length < 2) return (null, "Usage: power <who> [target]");
                var p = Own(t[1]);
                if (p == null) return (null, $"No player '{t[1]}' on your team.");
                var def = Catalog.Power(p.Power);
                if (def == null) return (null, $"{p.Label} has no power.");
                string arg = t.Length >= 3 ? t[2] : null;
                switch (def.Target)
                {
                    case PowerTarget.GoalRow:
                        return (GameAction.PowerAtGoal(p.Id, arg != null ? Row(arg) ?? 4 : BestRow(p, true)), null);
                    case PowerTarget.Tile:
                    case PowerTarget.PassTarget:
                        if (arg == null) return def.Id == PowerId.RiftTackle ? (GameAction.Power(p.Id), null) : (null, $"{def.Name} needs a target.");
                        var tile = TargetTile(arg);
                        if (tile == null) return (null, $"'{arg}' isn't a teammate or a tile.");
                        return (GameAction.Power(p.Id, tile.Value), null);
                    default:
                        return (GameAction.Power(p.Id), null);
                }
            }

            case "u":
            case "ult":
            case "ultimate":
            {
                if (t.Length >= 3)
                {
                    var a = Own(t[1]);
                    var b = Own(t[2]);
                    if (a == null || b == null) return (null, "Usage: ult <who> <whom>");
                    return (GameAction.Ultimate(a.Id, b.Id), null);
                }
                return (GameAction.Ultimate(), null);
            }

            default:
                return (null, $"Unknown command '{t[0]}'. Type 'help'.");
        }
    }

    int BestRow(Player p, bool unblockable)
    {
        Contests.BestGoalProbability(s, p, out int row, unblockable);
        return row;
    }

    static int? Row(string text)
    {
        switch (text.ToLowerInvariant())
        {
            case "top":
            case "3": return 3;
            case "mid":
            case "middle":
            case "centre":
            case "center":
            case "4": return 4;
            case "bottom":
            case "bot":
            case "5": return 5;
            default: return null;
        }
    }

    Player Own(string label) =>
        s.PlayersOf(s.ActiveSide).FirstOrDefault(p => string.Equals(p.Label, label, StringComparison.OrdinalIgnoreCase));

    Tile? TargetTile(string text)
    {
        var mate = Own(text);
        if (mate != null) return mate.Pos;
        return Tile.TryParse(text, out var tile) ? tile : null;
    }

    void PrintEvents(ActionResult result)
    {
        foreach (var e in result.Events)
        {
            if (e.Type == EventType.TurnStarted) continue;
            Console.WriteLine("  " + EventText.Describe(s, e));
        }
    }
}
