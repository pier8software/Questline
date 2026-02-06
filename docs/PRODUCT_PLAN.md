# Questline Product Plan

## Mission Statement

**Questline** brings the classic parser-driven text adventure to a new generation, combining the imagination-first gameplay of retro adventures with modern multiplayer and progression systems — culminating in a cooperative MUD where friends can quest together.

## Target Audience

Retro gamers and RPG enthusiasts who appreciate the imaginative freedom of text-based adventures.

## Goals

1. **Capture the magic of classic text adventures** — parser-based interaction, evocative prose, puzzle-solving, and exploration.

2. **Enable social adventure** — let players form parties and tackle quests together in real-time.

3. **Provide meaningful progression** — XP, levelling, gear, abilities, and tiered content that rewards investment.

4. **Build a sustainable platform architecture** — design from day one to support the eventual MUD, even while delivering simpler milestones.

5. **Keep it open and hackable** — open-source, well-documented, welcoming to contributors.

## Roadmap

### Phase 0: Foundation

> Build a playable single-player text adventure that proves the core loop works, using architecture that won't need to be thrown away when multiplayer arrives.

**Milestones:**

| Milestone | Deliverable |
|-----------|-------------|
| 0.1 | Project scaffolding, world model, room graph, basic `look` and `go` commands |
| 0.2 | Item model, `get`, `drop`, `inventory` commands |
| 0.3 | Content loading from JSON files, 5-room dungeon defined as data |
| 0.4 | Puzzle mechanics — `use` command, door/barrier unlocking |
| 0.5 | Character creation flow (name entry, default race/class) |
| 0.6 | Save/load functionality |
| 0.7 | Polish — help command, error handling, end-game detection |

**What the player experiences:**

1. Launch game
2. Create new character (enter name) or load existing character
3. Explore a 5-room dungeon via parser commands
4. Solve a simple puzzle to progress
5. Reach the end (treasure room, escape, etc.)
6. Save progress at any point, resume later

**Explicitly out of scope:**

- Combat / enemies / damage
- NPCs with dialogue
- Multiplayer / networking
- Progression (XP, levelling, shops)
- Terminal.Gui or any TUI framework

### Phase 1: Solo Adventure

> Polished single-player experience with one complete adventure and progression basics.

- Combat system with enemies and damage
- XP and levelling mechanics
- Character abilities based on class
- One complete, polished adventure
- Gear with stat-based properties

### Phase 2: Party Play

> Networking layer enabling friends to adventure together.

- Client/server architecture
- Party formation in a shared hub
- Shared quest instances
- Party inventory system
- Real-time command synchronisation

### Phase 3: MUD Platform

> Full multiplayer platform with persistent world and drop-in/drop-out gameplay.

- Persistent hub world (towns, shops, quest givers)
- Multiple tiered quests/adventures
- Economy (currency, shops, trading)
- Character progression unlocking higher-tier content
- Graceful disconnect/reconnect handling

## License

Open source (specific license TBD).
