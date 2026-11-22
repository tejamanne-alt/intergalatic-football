using System;
using System.Collections.Generic;

namespace IntergalacticFootball.Rules
{
    /// <summary>Applies one validated action to a state, rolling contests and recording events.</summary>
    internal sealed class Resolver
    {
        readonly MatchState s;
        readonly RulesConfig cfg;
        readonly IRollSource rolls;
        readonly List<MatchEvent> events;
        bool turnEnded;

        public Resolver(MatchState state, IRollSource rolls, List<MatchEvent> events)
        {
            s = state;
            cfg = state.Config;
            this.rolls = rolls;
            this.events = events;
        }

        public bool Run(GameAction a)
        {
            bool ok;
            switch (a.Type)
            {
                case ActionType.EndTurn:
                    EndTurn();
                    return true;
                case ActionType.Ultimate:
                    ok = Ultimate(a); // costs no action
                    break;
                default:
                    s.ActionsLeft--;
                    ok = Act(a);
                    break;
            }
            if (!turnEnded && !s.IsOver && s.ActionsLeft <= 0) EndTurn();
            return ok;
        }

        bool Act(GameAction a)
        {
            var p = s.GetPlayer(a.PlayerId);
            switch (a.Type)
            {
                case ActionType.Move: return Move(p, a.Path);
                case ActionType.Pass: return Pass(p, a.Target, false);
                case ActionType.Shoot: return Shoot(p, a.GoalRow, false);
                case ActionType.Tackle: return TackleCarrier(p, 0);
                case ActionType.Power: return Power(p, a);
                default: throw new InvalidOperationException("Unhandled action " + a.Type);
            }
        }

        // ---- Building blocks ----------------------------------------------------------------

        void Add(MatchEvent e) => events.Add(e);

        bool Contest(ContestInfo c)
        {
            int roll = rolls != null ? rolls.NextRoll() : s.NextRoll();
            bool ok = roll < c.Chance;
            Add(new MatchEvent
            {
                Type = EventType.Contest,
                PlayerId = c.AttackerId,
                OtherPlayerId = c.DefenderId,
                ContestKind = c.Kind,
                Chance = c.Chance,
                Roll = roll,
                Success = ok,
            });
            return ok;
        }

        void GainFlux(Side side, int amount)
        {
            int gained = s.AddFlux(side, amount);
            if (gained > 0) Add(new MatchEvent { Type = EventType.FluxGained, Side = side, Amount = gained });
        }

        Player BallWinnerFor(int id) => s.GetPlayer(id);

        // ---- Move ---------------------------------------------------------------------------

        bool Move(Player p, IReadOnlyList<Tile> path)
        {
            bool carrying = s.BallCarrierId == p.Id;
            bool anchored = s.Team(p.Side).Species.Passive == Passive.Anchored;
            int ignoresLeft = carrying && s.Team(p.Side).Species.Passive == Passive.Phasing ? 1 : 0;
            int handled = 0;
            var from = p.Pos;
            var segment = new List<Tile>();

            foreach (var t in path)
            {
                p.Pos = t;
                segment.Add(t);

                if (!carrying)
                {
                    if (s.BallIsLoose && s.LooseBallTile == t)
                    {
                        Flush(p, ref from, segment);
                        s.GiveBall(p);
                        Add(new MatchEvent { Type = EventType.PickedUp, PlayerId = p.Id, To = t });
                        return true;
                    }
                    continue;
                }

                // Stepping into an enemy zone: every enemy whose zone covers this tile contests,
                // and the move ends here even if the carrier beats them all.
                bool contested = false;
                foreach (var e in s.PlayersOf(p.Side.Opponent()))
                {
                    int bit = 1 << e.Id;
                    if ((handled & bit) != 0 || !Contests.InZone(e, t)) continue;
                    handled |= bit;
                    Flush(p, ref from, segment);

                    if (ignoresLeft > 0)
                    {
                        ignoresLeft--;
                        Add(new MatchEvent { Type = EventType.ZoneIgnored, PlayerId = p.Id, OtherPlayerId = e.Id, To = t });
                        continue;
                    }

                    contested = true;
                    if (Contest(new ContestInfo(ContestKind.Dribble, p.Id, e.Id, Contests.Dribble(s, p, e)))) continue;

                    if (anchored)
                    {
                        Add(new MatchEvent { Type = EventType.HeldOn, PlayerId = p.Id, OtherPlayerId = e.Id, To = t });
                        return false;
                    }
                    s.GiveBall(e);
                    Add(new MatchEvent { Type = EventType.Dispossessed, PlayerId = e.Id, OtherPlayerId = p.Id, To = t });
                    GainFlux(e.Side, cfg.FluxPerTackle);
                    return false;
                }
                if (contested) return true;
            }
            Flush(p, ref from, segment);
            return true;
        }

        void Flush(Player p, ref Tile from, List<Tile> segment)
        {
            if (segment.Count == 0) return;
            var to = segment[segment.Count - 1];
            Add(new MatchEvent { Type = EventType.Moved, PlayerId = p.Id, From = from, To = to, Path = segment.ToArray() });
            from = to;
            segment.Clear();
        }

        // ---- Pass ---------------------------------------------------------------------------

        bool Pass(Player p, Tile target, bool phased)
        {
            Add(new MatchEvent { Type = EventType.PassMade, PlayerId = p.Id, From = p.Pos, To = target });
            foreach (var c in Contests.Pass(s, p, target, phased))
            {
                if (Contest(c)) continue;
                var interceptor = BallWinnerFor(c.DefenderId);
                s.GiveBall(interceptor);
                Add(new MatchEvent { Type = EventType.Intercepted, PlayerId = interceptor.Id, OtherPlayerId = p.Id, To = interceptor.Pos });
                GainFlux(interceptor.Side, cfg.FluxPerTackle);
                return false;
            }

            var receiver = s.PlayerAt(target);
            if (receiver != null)
            {
                s.GiveBall(receiver);
                Add(new MatchEvent { Type = EventType.PassCompleted, PlayerId = receiver.Id, OtherPlayerId = p.Id, To = target });
                GainFlux(p.Side, cfg.FluxPerPass);
            }
            else
            {
                s.DropBall(target);
                Add(new MatchEvent { Type = EventType.PassIntoSpace, OtherPlayerId = p.Id, To = target });
            }
            return true;
        }

        // ---- Shoot --------------------------------------------------------------------------

        bool Shoot(Player p, int row, bool unblockable)
        {
            var defending = p.Side.Opponent();
            Add(new MatchEvent { Type = EventType.Shot, PlayerId = p.Id, From = p.Pos, To = Pitch.GoalTile(defending, row), Amount = row });

            foreach (var c in Contests.Shot(s, p, row, unblockable))
            {
                if (Contest(c)) continue;
                switch (c.Kind)
                {
                    case ContestKind.Block:
                    {
                        var blocker = BallWinnerFor(c.DefenderId);
                        s.GiveBall(blocker);
                        Add(new MatchEvent { Type = EventType.Blocked, PlayerId = blocker.Id, OtherPlayerId = p.Id, To = blocker.Pos });
                        GainFlux(blocker.Side, cfg.FluxPerTackle);
                        break;
                    }
                    case ContestKind.Save:
                    {
                        var keeper = BallWinnerFor(c.DefenderId);
                        s.GiveBall(keeper);
                        Add(new MatchEvent { Type = EventType.Saved, PlayerId = keeper.Id, OtherPlayerId = p.Id, To = keeper.Pos });
                        GainFlux(keeper.Side, cfg.FluxPerSave);
                        break;
                    }
                    default:
                        Add(new MatchEvent { Type = EventType.Missed, PlayerId = p.Id });
                        GoalKick(defending);
                        break;
                }
                return false;
            }

            var team = s.Team(p.Side);
            team.Score++;
            Add(new MatchEvent { Type = EventType.Goal, PlayerId = p.Id, Side = p.Side, Amount = team.Score });
            s.ResetForKickOff(defending);
            Add(new MatchEvent { Type = EventType.KickOff, Side = defending });
            EndTurn();
            return true;
        }

        /// <summary>After a miss the keeper restarts play; if the keeper is benched, the nearest defender does.</summary>
        void GoalKick(Side defending)
        {
            var keeper = s.Keeper(defending);
            if (s.IsOnPitch(keeper))
            {
                s.GiveBall(keeper);
                return;
            }
            Player nearest = null;
            var mouth = Pitch.GoalMouth(defending, Pitch.GoalCentreRow);
            foreach (var d in s.PlayersOf(defending))
                if (s.IsOnPitch(d) && (nearest == null || d.Pos.DistanceTo(mouth) < nearest.Pos.DistanceTo(mouth)))
                    nearest = d;
            if (nearest != null) s.GiveBall(nearest);
            else s.DropBall(mouth);
        }

        // ---- Tackle -------------------------------------------------------------------------

        bool TackleCarrier(Player p, int modifier)
        {
            var carrier = s.Carrier;
            bool ok = Contest(new ContestInfo(ContestKind.Tackle, p.Id, carrier.Id, Contests.Tackle(s, p, carrier, modifier)));
            if (ok)
            {
                s.GiveBall(p);
                Add(new MatchEvent { Type = EventType.TackleWon, PlayerId = p.Id, OtherPlayerId = carrier.Id, To = p.Pos });
                GainFlux(p.Side, cfg.FluxPerTackle);
            }
            else
            {
                Add(new MatchEvent { Type = EventType.TackleFailed, PlayerId = p.Id, OtherPlayerId = carrier.Id });
            }
            return ok;
        }

        // ---- Powers -------------------------------------------------------------------------

        bool Power(Player p, GameAction a)
        {
            var def = Catalog.Power(p.Power);
            s.Team(p.Side).Flux -= def.FluxCost;
            p.PowerCooldown = def.Cooldown;
            Add(new MatchEvent { Type = EventType.PowerUsed, PlayerId = p.Id, Power = def.Id, Amount = def.FluxCost });

            switch (def.Id)
            {
                case PowerId.GravityWell: return GravityWell(p);
                case PowerId.Shockwave: return Shockwave(p);
                case PowerId.MeteorStrike: return Shoot(p, a.GoalRow, true);
                case PowerId.Blink:
                    Teleport(p, a.Target);
                    return true;
                case PowerId.PhasePass: return Pass(p, a.Target, true);
                case PowerId.RiftTackle:
                {
                    var landing = a.Target;
                    if (!a.HasTarget) Validation.TryFindRiftLanding(s, p, s.Carrier, out landing);
                    if (landing != p.Pos) Teleport(p, landing);
                    return TackleCarrier(p, cfg.RiftTackleModifier);
                }
                default: throw new InvalidOperationException("Unhandled power " + def.Id);
            }
        }

        void Teleport(Player p, Tile to)
        {
            var from = p.Pos;
            p.Pos = to;
            Add(new MatchEvent { Type = EventType.Teleported, PlayerId = p.Id, From = from, To = to });
            if (s.BallIsLoose && s.LooseBallTile == to)
            {
                s.GiveBall(p);
                Add(new MatchEvent { Type = EventType.PickedUp, PlayerId = p.Id, To = to });
            }
        }

        bool GravityWell(Player p)
        {
            var start = s.BallTile;
            var holder = s.Carrier;
            var line = Pitch.Line(start, p.Pos);
            var end = start;
            Player receiver = null;
            int steps = Math.Min(cfg.GravityWellPull, line.Count);
            for (int i = 0; i < steps; i++)
            {
                end = line[i];
                receiver = s.PlayerAt(end);
                if (receiver != null) break;
            }

            if (receiver != null) s.GiveBall(receiver);
            else s.DropBall(end);
            Add(new MatchEvent { Type = EventType.BallPulled, PlayerId = p.Id, OtherPlayerId = holder != null ? holder.Id : -1, From = start, To = end });
            if (receiver != null) Add(new MatchEvent { Type = EventType.PickedUp, PlayerId = receiver.Id, To = end });
            return receiver == null || receiver.Side == p.Side;
        }

        bool Shockwave(Player p)
        {
            foreach (var e in s.PlayersOf(p.Side.Opponent()))
            {
                if (!s.IsOnPitch(e) || e.Pos.DistanceTo(p.Pos) != 1) continue;
                var from = e.Pos;
                var to = from.Offset(Math.Sign(from.X - p.Pos.X), Math.Sign(from.Y - p.Pos.Y));
                if (!s.CanStandOn(e, to) || s.PlayerAt(to) != null)
                {
                    Add(new MatchEvent { Type = EventType.PushBlocked, PlayerId = e.Id, OtherPlayerId = p.Id, From = from });
                    continue;
                }

                bool hadBall = s.BallCarrierId == e.Id;
                e.Pos = to;
                Add(new MatchEvent { Type = EventType.Pushed, PlayerId = e.Id, OtherPlayerId = p.Id, From = from, To = to });
                if (hadBall && s.Team(e.Side).Species.Passive != Passive.Anchored)
                {
                    s.DropBall(from);
                    Add(new MatchEvent { Type = EventType.BallDropped, PlayerId = e.Id, To = from });
                }
                else if (!hadBall && s.BallIsLoose && s.LooseBallTile == to)
                {
                    s.GiveBall(e);
                    Add(new MatchEvent { Type = EventType.PickedUp, PlayerId = e.Id, To = to });
                }
            }
            return true;
        }

        // ---- Ultimates ----------------------------------------------------------------------

        bool Ultimate(GameAction a)
        {
            var team = s.Team(s.ActiveSide);
            var id = team.Species.Ultimate;
            team.Flux = 0;
            team.UltimateUsed = true;
            Add(new MatchEvent { Type = EventType.UltimateUsed, Side = team.Side, Ultimate = id });

            switch (id)
            {
                case UltimateId.Crush:
                {
                    var opponent = s.Team(team.Side.Opponent());
                    opponent.PaceHalvedThroughTurn = s.NextTurnOf(opponent.Side, s.TurnIndex);
                    Add(new MatchEvent { Type = EventType.PaceHalved, Side = opponent.Side, Amount = opponent.PaceHalvedThroughTurn });
                    return true;
                }
                case UltimateId.FoldSpace:
                {
                    var first = s.GetPlayer(a.PlayerId);
                    var second = s.GetPlayer(a.OtherPlayerId);
                    var t = first.Pos;
                    first.Pos = second.Pos;
                    second.Pos = t;
                    Add(new MatchEvent { Type = EventType.Swapped, PlayerId = first.Id, OtherPlayerId = second.Id, From = second.Pos, To = first.Pos });
                    return true;
                }
                default: throw new InvalidOperationException("Unhandled ultimate " + id);
            }
        }

        // ---- Turn flow ----------------------------------------------------------------------

        void EndTurn()
        {
            turnEnded = true;
            Add(new MatchEvent { Type = EventType.TurnEnded, Side = s.ActiveSide, Amount = s.TurnIndex });
            s.TurnIndex++;

            if (s.TurnIndex >= cfg.TotalTurns)
            {
                s.IsOver = true;
                s.ActionsLeft = 0;
                Add(new MatchEvent { Type = EventType.FullTime });
                return;
            }

            if (s.TurnIndex == cfg.TurnsPerHalf)
            {
                Add(new MatchEvent { Type = EventType.HalfTime });
                s.ResetForKickOff(Side.Away);
                Add(new MatchEvent { Type = EventType.KickOff, Side = Side.Away });
            }

            s.BeginTurn();
            Add(new MatchEvent { Type = EventType.TurnStarted, Side = s.ActiveSide, Amount = s.TurnIndex });
        }
    }
}
