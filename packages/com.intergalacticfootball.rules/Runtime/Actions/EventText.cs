namespace IntergalacticFootball.Rules
{
    /// <summary>Plain-English descriptions of events, for logs, debug overlays and the terminal harness.</summary>
    public static class EventText
    {
        public static string Describe(MatchState s, MatchEvent e)
        {
            string P(int id) => id >= 0 ? s.GetPlayer(id).Label : "?";
            string Team(Side side) => s.Team(side).Species.Name;

            switch (e.Type)
            {
                case EventType.TurnStarted: return $"{Team(e.Side)} to play.";
                case EventType.TurnEnded: return $"{Team(e.Side)} end their turn.";
                case EventType.KickOff: return $"{Team(e.Side)} kick off.";
                case EventType.HalfTime: return "Half time.";
                case EventType.FullTime: return "Full time.";
                case EventType.Moved: return $"{P(e.PlayerId)} moves to {e.To}.";
                case EventType.ZoneIgnored: return $"{P(e.PlayerId)} phases through {P(e.OtherPlayerId)}'s zone.";
                case EventType.Contest:
                {
                    string versus = e.OtherPlayerId >= 0 ? $" vs {P(e.OtherPlayerId)}" : "";
                    return $"  {e.ContestKind}: {P(e.PlayerId)}{versus} at {e.Chance}%, rolled {e.Roll}: {(e.Success ? "success" : "fail")}.";
                }
                case EventType.Dispossessed: return $"{P(e.PlayerId)} wins the ball off {P(e.OtherPlayerId)}!";
                case EventType.HeldOn: return $"{P(e.PlayerId)} is stopped by {P(e.OtherPlayerId)} but holds on to the ball.";
                case EventType.PickedUp: return $"{P(e.PlayerId)} picks up the ball.";
                case EventType.PassMade: return $"{P(e.PlayerId)} passes toward {e.To}.";
                case EventType.PassCompleted: return $"{P(e.PlayerId)} receives.";
                case EventType.PassIntoSpace: return $"The ball runs into space at {e.To}.";
                case EventType.Intercepted: return $"Intercepted by {P(e.PlayerId)}!";
                case EventType.TackleWon: return $"{P(e.PlayerId)} tackles {P(e.OtherPlayerId)} and wins the ball!";
                case EventType.TackleFailed: return $"{P(e.OtherPlayerId)} rides {P(e.PlayerId)}'s tackle.";
                case EventType.Shot: return $"{P(e.PlayerId)} shoots at row {e.Amount}!";
                case EventType.Goal: return $"GOAL! {P(e.PlayerId)} scores for {Team(e.Side)}.";
                case EventType.Saved: return $"Saved by {P(e.PlayerId)}!";
                case EventType.Blocked: return $"Blocked by {P(e.PlayerId)}!";
                case EventType.Missed: return $"{P(e.PlayerId)} misses the target.";
                case EventType.PowerUsed: return $"{P(e.PlayerId)} uses {Catalog.Power(e.Power).Name} (-{e.Amount} Flux).";
                case EventType.UltimateUsed: return $"{Team(e.Side)} unleash {Catalog.Ultimate(e.Ultimate).Name.ToUpperInvariant()}!";
                case EventType.Teleported: return $"{P(e.PlayerId)} blinks to {e.To}.";
                case EventType.Pushed: return $"{P(e.PlayerId)} is shoved to {e.To}.";
                case EventType.PushBlocked: return $"{P(e.PlayerId)} has nowhere to be pushed.";
                case EventType.BallDropped: return $"{P(e.PlayerId)} loses the ball at {e.To}.";
                case EventType.BallPulled: return $"The ball is pulled to {e.To}.";
                case EventType.Swapped: return $"{P(e.PlayerId)} and {P(e.OtherPlayerId)} swap places.";
                case EventType.PaceHalved: return $"{Team(e.Side)}' Pace is halved until the end of their next turn.";
                case EventType.FluxGained: return $"  +{e.Amount} Flux for {Team(e.Side)}.";
                default: return e.Type.ToString();
            }
        }
    }
}
