# Intergalactic Football: Core Concept

Nov 22, 2026

## Pitch

Intergalactic Football is 7v7 football played as a turn-based tactics game on a grid. Each team is a species, human or alien, with its own powers, and each match is on a planet whose physics changes the rules.

Matches are short, 6 to 8 minutes, and played in landscape with the full pitch always visible.

## The turn system

Each team gets 3 actions per turn, shared by the whole team. You choose who acts: one player can use all three, or three players can use one each. This keeps turns fast and makes every action a real choice.

- **Pitch:** a 15×9 tile grid, with goals 3 tiles wide.
- **Actions:** Move, Pass, Shoot, Tackle, Power.
- **Zones of control:** every player threatens the tiles next to them. A pass or dribble through an enemy zone triggers a contest (Pass vs Tackle stat plus a small roll). The success percentage is shown before you commit.
- **Match length:** two halves of 10 turns per side.

## Squad of 7

Every squad has the same seven roles, and each player has four stats: Pace, Pass, Shot and Tackle.

| Role | Count | Job |
| --- | --- | --- |
| Keeper | 1 | Guards the goal |
| Anchor | 2 | Defence |
| Playmaker | 2 | Passing |
| Runner | 1 | Pace |
| Striker | 1 | Shooting |

Formations such as 2-3-1, 3-2-1 or 1-3-2 are chosen before kick-off.

## Special powers

Successful passes, tackles and saves fill a **Flux** meter, which you spend on powers. Powers come in three layers:

1. **Species passive:** always on, and defines the team's style.
2. **Player power:** one per player, costs Flux and has a cooldown.
3. **Team ultimate:** once per match, at full Flux.

| Species | Style | Passive | Example power | Ultimate |
| --- | --- | --- | --- | --- |
| Gravions | Power | Can't be pushed off the ball | Gravity Well: pull the ball 2 tiles toward you | Crush: enemy Pace halved for a turn |
| Blinkers | Speed | Ignore one enemy zone per move | Blink: teleport 3 tiles | Fold Space: swap any two of your players |
| Chronites | Control | See contest odds 5% in your favour | Rewind: undo your last action | Time Stop: take 5 actions this turn |
| Sporekin | Traps | Leave slowing spores on tiles you exit | Root: pin an enemy for a turn | Bloom: all spore tiles block passes |
| Humans | All-round | One pass to an adjacent teammate per turn costs no action | Slide Tackle: tackle from 2 tiles away | Total Football: every player moves 1 tile for free |

## Planets

Each stadium has one rule that changes how you play.

| Planet | Rule |
| --- | --- |
| Low-gravity moon | Long passes fly over one line of defenders |
| Ice world | Players slide one extra tile after moving |
| Lava planet | Marked tiles erupt on a visible countdown |
| Wormhole arena | Two linked portals carry the ball or players across the pitch |

## Strategic and tactical layers

Strategy happens before the match and tactics happen during it.

- **Strategic (before the match):** species, formation, which 7 powers to bring, and countering the planet rule.
- **Tactical (during the match):** positioning around zones, spending 3 actions well, and timing Flux. For example, save for the ultimate or spend now to break a press.

## Modes

- **Galactic Cup:** a campaign across the planets, unlocking species and powers.
- **Puzzles:** "score in 2 turns" challenges that also work as the tutorial.
- **Pass-and-play and async PvP:** both suit turn-based play.

## Tech stack

Build it in Unity with C#: one project exports to both iOS and Android. Add Supabase only when async PvP arrives; everything before that runs on the device.

[Unity Personal](https://unity.com/products/unity-personal) is free while revenue and funds raised stay under $200K USD in the last 12 months.

| Layer | Choice | Why |
| --- | --- | --- |
| Engine | [Unity 6.3 LTS](https://unity.com/releases/unity-6/support) (2D), C# | One codebase for iOS and Android; this LTS is supported until December 2027 |
| Pitch and effects | Tilemap for the 15×9 grid, a tween library such as DOTween for moves, Particle System for powers | Tilemap and particles are built in; tweening is one package |
| Rules engine | A plain C# assembly with no UnityEngine references: match state, action resolver, seeded random roll | The same code drives matches, the AI, puzzles and replays, and can also run on a server |
| AI opponent | Scores every legal action, with a shallow look-ahead across the 3 actions | Runs on the device; difficulty is tuned by look-ahead depth |
| Content data | Species, powers and planets as ScriptableObjects | New content is added as data, not code |
| Local save | JSON save file on the device | Campaign and puzzle progress work offline |
| Tests | Unity Test Framework edit-mode tests on the rules assembly | Power interactions are where bugs will hide |
| Backend (async PvP) | Supabase through its community C# client: Postgres for matches and turns, Auth for accounts, Realtime for turn updates | Turn-based netcode is "write turn, notify opponent" |
| Turn validation (async PvP) | A [Cloud Code C# module](https://docs.unity.com/en-us/cloud-code/modules/overview) that runs the rules assembly | The server checks every turn with the same code the game uses |
| Push (async PvP) | Firebase Cloud Messaging through the Firebase Unity SDK | "Your turn" alerts on both platforms |
| Build and release | IL2CPP builds, Xcode and TestFlight for iOS, AAB and Play internal testing for Android | Both store tracks from the first playable build |
| Source control | Git on GitHub, with Git LFS for art and audio |  |

One rule to hold from day one: keep the rules assembly free of UnityEngine classes. Cloud Code modules run on the open .NET runtime and cannot use them, so an engine-free assembly is what lets the server validate turns.

## First build

Start with one neutral stadium, two species (Gravions and Blinkers), six powers, and a match against AI plus pass-and-play.

The main risk is pacing: football is fluid and turns can feel slow. The 3-action limit and fast animations are the fix, so test that first.

## Open question

- [ ] Is this mainly a single-player game against AI, or is PvP the main mode? The answer decides how deep the campaign and the power balance need to go.
