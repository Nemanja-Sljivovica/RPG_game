# RPG Game - Nordeus Full Stack Challenge 2026

A turn-based fantasy RPG where a knight fights through 5 monsters in sequence, learning new moves from each defeated enemy. Built with Unity 6 on the client and Django on the server.

Made by Nemanja Mijatovic for the Nordeus Job Fair 2026 Full Stack challenge.

## How to run

You need Python 3.10+ and Unity 6 (6000.4.5f1) with the URP 2D template support.

### 1. Start the server

Open a terminal in the `server/` folder:
```
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
python manage.py runserver
```

The server starts on `http://localhost:8000`. Leave the terminal open during play.

To verify it works, open `http://localhost:8000/api/run/config/` in a browser. You should see a JSON payload with hero stats, all 5 monsters, and the move catalog.

### 2. Start the client

Open the `RPG_game` folder in Unity Hub, then open `Assets/Scenes/MainMenu.unity` and press Play. Click "Start Run" to begin a run.

## Game flow

The hero starts with 4 default moves (Slash, Shield Up, Battle Cry, Second Wind) and faces a fixed sequence of 5 monsters: Goblin Warrior → Giant Spider → Goblin Mage → Witch → Dragon. Combat is turn-based: pick a move (click a button or type its 4-character WASD sequence), the server picks the monster's response, both moves resolve, repeat until one side hits 0 HP.

Winning a fight awards XP and a 100% chance to learn one of the monster's moves at random (if any are still unlearned). Losing returns the player to the map with no progress lost - the fight can be retried freely. Beaten monsters can also be replayed for half XP and another chance at learning their remaining moves.

The Move Management screen lets the player swap any learned move into the 4 equipped slots before each fight.

## Architecture

The server is stateless and exposes two GET endpoints:

- `GET /api/run/config/` - called once when a run starts. Returns hero stats, level-up curve, full move catalog, and all 5 monsters in order.
- `GET /api/battle/monster-move/?monster_id=X&monster_hp_pct=Y&hero_hp_pct=Z` - called after every player turn. Returns which move the monster plays, picked by a weighted random AI that prioritizes finishing damage when the hero is low and defensive buffs when the monster is hurt.

The client holds combat state during a fight (HP, active buffs, turn counter) and applies damage locally using the formulas defined in `MoveResolver.cs`. The monster's choice of move is always server-authoritative - the client never decides what the monster does, only how the move resolves visually.

Game data lives in `server/game/config.py`. Designers can change monster stats, move power, AI weights, the level-up curve - anything in that file - and restart the server to see the changes in the next run, no client rebuild required.

## Project structure

```
RPG_game/
├── Assets/
│   ├── Scenes/              MainMenu, Map, Battle, MoveManagement
│   ├── Scripts/
│   │   ├── Data/            GameData, GameState, SpriteRegistry
│   │   ├── Network/         ApiClient (UnityWebRequest + Newtonsoft.Json)
│   │   ├── Battle/          BattleController, Combatant, MoveResolver
│   │   ├── Map/             MapController
│   │   └── UI/              MainMenuController, MoveManagementController
│   └── Sprites/             Character spritesheets and backgrounds
├── server/
│   ├── rpgserver/           Django project settings + root URL config
│   ├── game/                App with config.py (game data), views.py, urls.py
│   └── manage.py
└── README.md
```

## Design decisions

**Stateless server.** The server doesn't track ongoing fights - the client sends current state with every monster-move request. This keeps the server simple, matches the brief's "two GET endpoints" requirement cleanly, and makes the system easier to reason about.

**Game data as Python config, not database.** All monster stats, move definitions, AI weights, and balance numbers live in a single `config.py` file. A designer can edit it directly. The brief asked for fast tweaking of game logic without a client rebuild - this delivers that with the least friction. If a database becomes needed later, migration to Django models is straightforward.

**Damage math on the client, AI on the server.** The brief asked for game logic to live server-side. I put the data and the bot decision-making on the server but kept damage resolution on the client to avoid an HTTP round-trip per move. A move's stats (type, power, scaling) come from the server; how those stats turn into a number is computed locally. A future iteration could move resolution server-side via a third endpoint with no architectural change.

**WASD combo input layer on top of click-to-cast.** Each move has a unique 4-character keyboard sequence. Players can either click a move button or type the sequence - buttons stay available so reviewers and casual players don't need to memorize anything, and the keyboard layer adds a "power-user" feel for engaged players.

**Default Knight hero.** The brief allows substituting the hero. I stayed with the Knight to fully match the brief's reference moveset and keep the system identifiable.

## Bonus features implemented

From the GD's bonus list:

- **Smarter bot.** Monster AI weights moves situationally - more likely to attack when the hero is below 30% HP, more likely to use defensive buffs when its own HP is below 40%.
- **Replay any beaten monster.** Click any green-tinted slot on the map to refight that monster. Replays award half XP and still have a chance at teaching another of that monster's moves.
- **Battle log.** A running text feed shows what each side just did each turn.
- **Battle animations.** Sprites lunge forward when attacking, flash red when hit, smoothly drain HP bars, and fade out on defeat.
- **Visual polish.** Color-coded move buttons (orange for physical, blue for magic), encounter slot tinting (green for beaten, yellow for current, gray for future), themed backgrounds for menu / map / battle scenes.

## Known limitations

- Combat resolution is split between server (data + AI) and client (math). A fully server-authoritative version would resolve every move on the server too. Documented above as a future iteration.
- No save/load between sessions. A run is in-memory only; closing the client loses progress.
- Single hero class. The brief's bonus list mentions class selection - out of scope for this submission.
