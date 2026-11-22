# Rules v0.1: what the engine implements

This is the concrete ruleset behind the [core concept](concept.md). The concept left many details open. Where it did, this page says what the engine does and why, so each choice can be argued with and changed. Every number lives in `RulesConfig` (rules) or `Catalog` (species, powers), so tuning is a data change.

Scope is the concept's **first build**: one neutral stadium, Gravions and Blinkers, six powers, a match against AI, and pass-and-play.

## Pitch

- 15×9 tiles. X runs 0–14 from the Home goal to the Away goal; Y runs 0–8.
- Goals are 3 tiles wide (rows 3–5) and sit just off the pitch: Home's at x = −1, Away's at x = 15.
- Penalty areas are 3 tiles deep and cover rows 1–7. **Keepers can't leave their penalty area.**
- Distances are Chebyshev: diagonal steps count as one, and "adjacent" means the 8 surrounding tiles.

## Turns

- Each turn the side to act has **3 actions**, shared by the whole team. One player can take all three.
- Actions: **Move, Pass, Shoot, Tackle, Power**, each costing 1 action. You can also end the turn early.
- The team **ultimate costs no action**.
- A match is two halves of 10 turns per side (40 team-turns).
- Home kicks off and acts first in the first half; Away does both in the second half.
- **A goal ends the scorer's turn.** Positions reset and the conceding side kicks off, which is also whose turn comes next.
- **Losing the ball does not end your turn.** You can use your remaining actions to win it back.
- A kick-off puts the striker on the centre spot with the ball. The other side is at least 2 tiles away.

## Players

Every squad has the same 7 slots: Keeper **K**, Anchors **A B**, Playmakers **P Q**, Runner **R**, Striker **S**. The board shows Home in upper case and Away in lower case.

Stats are on a 1–10 scale. Three of them set a range:

| Stat | Used for | Range rule |
| --- | --- | --- |
| Pace | Tiles per Move; dribbles and resisting tackles | Move range = 1 + Pace/3 (3–5 → 2, 6–8 → 3, 9–10 → 4) |
| Pass | Passing and beating interceptions | Pass range = 4 + Pass/2 |
| Shot | Shooting | Shot range = 3 + Shot/2, measured to the goal tile |
| Tackle | **Every defensive contest.** A keeper's Tackle is their save rating | none |

Base stats by role (Pace / Pass / Shot / Tackle): Keeper 4/5/2/8, Anchor 4/4/3/7, Playmaker 5/8/4/5, Runner 8/5/5/4, Striker 6/4/8/3.

## Zones and contests

Every player threatens their own tile and the 8 around it. **Zones only affect the ball:** players without it move freely.

Every contest is one formula, and the preview shows the result before you commit:

```
chance = clamp(50 + 8 × (attacking stat − Tackle) + modifiers, 5%, 95%)
roll a d100 (0–99); success if roll < chance
```

| Contest | When | Attacker vs defender | Modifiers |
| --- | --- | --- | --- |
| Dribble | Carrier steps onto a tile in an enemy zone | Pace vs Tackle | none |
| Tackle | Tackle action next to the carrier | Tackle vs carrier's Pace | Rift Tackle +10 |
| Interception | Pass travels through an enemy zone, the receiver's tile included | Pass vs Tackle | +10 if the defender only marks the lane, −10 if they stand in it; −3 per tile beyond 4 |
| Block | Shot travels through an outfield defender's zone | Shot vs Tackle | 0 if marking the lane, −20 if standing in it |
| Save | Keeper within 2 tiles of the goal-mouth tile aimed at | Shot vs keeper's Tackle | −10 keeper edge; −4 per tile beyond 3 (+4/+8 when closer); +20 if the keeper is 2 tiles away |
| Accuracy | No keeper within reach | none | 95%, −5 per tile beyond 3, at least 50% |

- **Dribbling (key rule): stepping into an enemy zone with the ball ends the move**, win or lose. Every enemy whose zone covers that tile contests once.
  - Lose a contest and that defender takes the ball (+2 Flux to them).
  - Moving out of zones is free, so beating your man costs a Move and passing is the fast way up the pitch.
- **Passes** target a tile. If a teammate stands there they receive it. If the tile is empty the ball runs loose there (a through ball). Each defender contests at most once, in the order the ball meets them. If one wins, they take the ball.
- **Shots** pick a goal row (3–5).
  - Blocks are rolled first. A block gives the blocker the ball.
  - Then comes the save, or the accuracy roll if no keeper is in reach. A failed save gives the keeper the ball; a miss is a goal kick to the keeper.
- **Loose ball:** moving onto it picks it up and ends that move.

## Flux, powers and ultimates

- **Flux** (team meter, max 10):
  - A completed pass gives +1.
  - Winning the ball gives +2: a tackle, an interception, a block, or beating a dribbler. So does a save.
  - A pass into space gives nothing until someone collects it.
- **Player powers** cost Flux and 1 action, and have a cooldown counted in the owner's turns. Cooldown 2 means you skip your next turn and can use the power the turn after.
- **Ultimate:** needs full Flux, once per match, spends all Flux, **costs no action**.

### Gravions (Power)

Stat modifier: Shot +1, Tackle +1 on outfield players. Passive **Anchored** ("can't be pushed off the ball"): a carrier who loses a dribble contest keeps the ball, and Shockwave can't knock it loose.

| Power | Flux | Cooldown | Effect | Default players |
| --- | --- | --- | --- | --- |
| Gravity Well | 4 | 3 | Pull the ball 2 tiles toward you from up to 4 tiles away, even off a carrier's feet. It stops at the first player in the way, who gets it. | K, P, Q |
| Shockwave | 3 | 2 | Push every adjacent opponent 1 tile directly away. A pushed carrier drops the ball where they stood. Blocked pushes do nothing. | A, B, R |
| Meteor Strike | 4 | 3 | A shot outfield players can't block. The keeper still saves as normal. | S |
| **Crush** (ultimate) | full | once | Opponents' Pace is halved until the end of their next turn. This also helps your own tackles this turn. | |

### Blinkers (Speed)

Stat modifier: Pace +1, Tackle −1 on outfield players. Passive **Phasing**: a ball carrier ignores the first enemy zone they enter on each move: no contest, no stop.

| Power | Flux | Cooldown | Effect | Default players |
| --- | --- | --- | --- | --- |
| Blink | 3 | 3 | Teleport up to 3 tiles, taking the ball along. No contests. | K (inside the box), R, S |
| Phase Pass | 3 | 2 | A pass that can't be intercepted. | P, Q |
| Rift Tackle | 3 | 3 | Teleport next to an enemy carrier within 3 tiles and tackle at +10%. | A, B |
| **Fold Space** (ultimate) | full | once | Swap any two of your players. Keepers can't be swapped out of the box. | |

## Formations

Kick-off positions in Home orientation; Away is mirrored. Outfield players start at x ≤ 5, so the defending side is never next to the kick-off.

| Formation | K | A | B | P | Q | R | S |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 2-3-1 | 0,4 | 2,2 | 2,6 | 4,4 | 4,1 | 4,7 | 5,4 |
| 3-2-1 | 0,4 | 2,2 | 2,6 | 2,4 | 4,3 | 4,6 | 5,4 |
| 1-3-2 | 0,4 | 2,4 | 4,4 | 3,1 | 3,7 | 5,2 | 5,6 |

## Decisions where the concept was open

| Question | Decision | Why |
| --- | --- | --- |
| Do zones stop players without the ball? | No, zones only affect the ball | Keeps off-ball runs fluid; marking still matters because receivers in a zone are contested |
| Does winning a dribble let you keep going? | **No: entering a zone ends the move** | In the first sims a striker could dribble through two zones and score straight from kick-off. Goals were 9 a match, with a shot in half of all turns |
| Which stats meet in a contest? | Attacking stat (Pace, Pass or Shot) vs the defender's Tackle | One rule to learn; Tackle is the single defensive stat |
| How do keepers save? | Keeper's Tackle is the save rating, and the keeper must be within 2 tiles of where the shot is aimed | Keeper positioning matters, with no fifth stat |
| "Can't be pushed off the ball" | Anchored: losing a dribble or being shoved by Shockwave never costs Gravions the ball | Immunity to tackles would be unbeatable |
| Do ultimates cost an action? | No | Fits "Time Stop: take 5 actions" and "Total Football: … for free" |
| Does losing the ball end the turn? | No | Lets you press to win it back (counter-pressing) |
| Kick-off after half time | Away | Each side kicks off one half |
| Species stat modifiers on keepers? | Not applied | A single point of keeper Tackle moved every save by 8% and swamped the balance |
| Randomness | PCG32 with the seed and roll stream inside the match state | System.Random differs between runtimes; replays and server checks need identical rolls |

## What the sims say

Run `dotnet run -c Release --project dotnet/Cli -- sim --matches 200`. The medium AI played both sides, and species swapped ends every other match.

| Metric | Result |
| --- | --- |
| Goals per match | about 4.0 (common scores 2-2, 1-1, 2-1) |
| Turns with a shot | about 33% |
| Actions used per turn | 2.93 of 3 |
| Shot conversion | 27–35% |
| AI time per action, medium | about 3 ms on average, under 100 ms worst case (Release build, cloud container) |
| Home / draw / away | 66 / 65 / 69 |
| Gravions / draw / Blinkers | 77 / 65 / 58 |

How the balance got here: the first pass had Gravions at Pace −1. They lost 50 to 99, because Pace thresholds gave Blinkers an extra tile of movement on three roles. Moving the Gravion modifier to Shot +1, Tackle +1 brought it to the numbers above. Blink also went from 2 Flux with a 2-turn cooldown to 3 and 3, after it was used 3.6 times a match.

## Known gaps

- **Hard AI is not reliably stronger than Medium.**
  - Medium beats Easy 28–18 over 60 mirror matches.
  - Hard against Medium is 26–22, which is inconclusive. Hard also costs about 9 ms an action on average and up to 500 ms in the worst case.
  - Looking 3 actions ahead instead of 2 doesn't help yet: the position evaluator (`Ai/Evaluator.cs`) is the limit, not search depth. Improve it before relying on depth for difficulty.
- **Gravions have a slight edge:** 77 to 58 over 200 matches, about 1.6 standard deviations. That is worth another tuning pass with more matches.
- **The AI barely uses some tools:** Shockwave (about 0.4 a match) and accuracy rolls (keepers are almost always in reach). These may be AI blind spots rather than weak rules; test them by hand.
- **Not built yet:** planets (only the neutral stadium), Chronites, Sporekin and Humans, puzzles, and the campaign. `RulesConfig` and the `Passive`, `PowerId` and `UltimateId` hooks are where they plug in.
- **Async PvP security:** the roll seed is part of the match state, so a client that holds it can predict rolls. For PvP the server must own the seed (or use commit-reveal), so the client only sees results.
