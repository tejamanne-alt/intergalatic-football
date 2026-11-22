using System;
using System.Text;
using IntergalacticFootball.Rules;

namespace IntergalacticFootball.Cli;

/// <summary>Draws the pitch, scoreboard and squads as text.</summary>
static class Board
{
    const string Reset = "\u001b[0m";
    const string HomeColor = "\u001b[96m";
    const string AwayColor = "\u001b[93m";
    const string Dim = "\u001b[90m";
    const string Bold = "\u001b[1m";

    public static string Render(MatchState s, bool color)
    {
        var sb = new StringBuilder();
        string C(string code, string text) => color ? code + text + Reset : text;
        var home = s.Team(Side.Home);
        var away = s.Team(Side.Away);

        string half = s.Half == 1 ? "1st half" : "2nd half";
        string status = s.IsOver
            ? "Full time"
            : $"{half}, turn {s.TurnOfHalf}/{s.Config.TurnsPerHalfPerSide}  |  {s.Team(s.ActiveSide).Species.Name} to play, {s.ActionsLeft} action{(s.ActionsLeft == 1 ? "" : "s")} left";
        sb.AppendLine();
        sb.AppendLine($"  {C(HomeColor + Bold, home.Species.Name.ToUpperInvariant())} {home.Score} - {away.Score} {C(AwayColor + Bold, away.Species.Name.ToUpperInvariant())}    {status}");
        sb.AppendLine($"  Flux  {C(HomeColor, FluxBar(s, Side.Home))}   {C(AwayColor, FluxBar(s, Side.Away))}");
        sb.AppendLine();

        sb.Append("     ");
        for (int x = 0; x < Pitch.Width; x++) sb.Append(x.ToString().PadLeft(3));
        sb.AppendLine();

        for (int y = 0; y < Pitch.Height; y++)
        {
            bool goalRow = Pitch.IsGoalRow(y);
            sb.Append(y.ToString().PadLeft(4));
            sb.Append(goalRow ? C(HomeColor, " #") : "  ");
            for (int x = 0; x < Pitch.Width; x++)
            {
                var t = new Tile(x, y);
                var p = s.PlayerAt(t);
                string cell;
                if (p != null)
                {
                    string code = p.Side == Side.Home ? HomeColor : AwayColor;
                    cell = s.BallCarrierId == p.Id ? C(code + Bold, $"[{p.Label}]") : C(code, $" {p.Label} ");
                }
                else if (s.BallIsLoose && s.LooseBallTile == t)
                {
                    cell = C(Bold, " o ");
                }
                else
                {
                    bool box = Pitch.InPenaltyArea(Side.Home, t) || Pitch.InPenaltyArea(Side.Away, t);
                    cell = C(Dim, t == Pitch.Centre ? " + " : box ? " , " : " . ");
                }
                sb.Append(cell);
            }
            sb.Append(goalRow ? C(AwayColor, "#") : "");
            sb.AppendLine();
        }
        sb.AppendLine(C(Dim, "     [X] has the ball   o loose ball   , penalty area   # goal   UPPER = home, lower = away"));
        return sb.ToString();
    }

    static string FluxBar(MatchState s, Side side)
    {
        var team = s.Team(side);
        int max = s.Config.MaxFlux;
        string bar = new string('#', team.Flux) + new string('-', Math.Max(0, max - team.Flux));
        string ult = team.UltimateUsed ? " ultimate used" : s.UltimateReady(side) ? " ULTIMATE READY" : "";
        return $"{team.Species.Name} [{bar}] {team.Flux}/{max}{ult}";
    }

    public static string Squads(MatchState s)
    {
        var sb = new StringBuilder();
        foreach (var side in new[] { Side.Home, Side.Away })
        {
            var team = s.Team(side);
            var ult = Catalog.Ultimate(team.Species.Ultimate);
            sb.AppendLine($"  {team.Species.Name} ({team.Formation.Name}). Passive {team.Species.PassiveName}: {team.Species.PassiveDescription}");
            sb.AppendLine($"  Ultimate {ult.Name}: {ult.Description}");
            sb.AppendLine("     Role       Pace(move) Pass(range) Shot(range) Tackle  Power");
            foreach (var p in s.PlayersOf(side))
            {
                var power = Catalog.Power(p.Power);
                string powerText = power == null ? "-" : $"{power.Name} ({power.FluxCost} Flux{(p.PowerCooldown > 0 ? $", ready in {p.PowerCooldown}" : "")})";
                int pace = s.EffectivePace(p);
                sb.AppendLine($"  {p.Label,2} {p.Role,-10} {pace,4}({s.MoveRange(p)})    {p.Stats.Pass,4}({s.PassRange(p)})     {p.Stats.Shot,4}({s.ShotRange(p)})     {p.Stats.Tackle,4}    {powerText}");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static string DescribeAction(MatchState s, GameAction a)
    {
        string L(int id) => s.GetPlayer(id).Label;
        switch (a.Type)
        {
            case ActionType.Move: return $"{L(a.PlayerId)} moves to {a.Path[a.Path.Count - 1]}";
            case ActionType.Pass:
            {
                var receiver = s.PlayerAt(a.Target);
                return receiver != null ? $"{L(a.PlayerId)} passes to {receiver.Label}" : $"{L(a.PlayerId)} plays it into space at {a.Target}";
            }
            case ActionType.Shoot: return $"{L(a.PlayerId)} shoots at row {a.GoalRow}";
            case ActionType.Tackle: return $"{L(a.PlayerId)} tackles {s.Carrier?.Label}";
            case ActionType.Power:
            {
                var p = s.GetPlayer(a.PlayerId);
                var def = Catalog.Power(p.Power);
                string target = a.HasTarget ? $" at {a.Target}" : def.Target == PowerTarget.GoalRow ? $" at row {a.GoalRow}" : "";
                return $"{p.Label} uses {def.Name}{target}";
            }
            case ActionType.Ultimate:
            {
                var def = Catalog.Ultimate(s.Team(s.ActiveSide).Species.Ultimate);
                return def.NeedsTwoPlayers ? $"{def.Name}: swap {L(a.PlayerId)} and {L(a.OtherPlayerId)}" : def.Name;
            }
            default: return "End turn";
        }
    }
}
