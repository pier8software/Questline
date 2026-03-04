## Context

The game currently saves playthrough state to MongoDB on every meaningful action (move, take, drop, character creation), but there is no way to resume a saved playthrough. After login, the engine moves straight to adventure selection and character creation. The `Playthrough` entity and its MongoDB document do not currently store a player/username association, so there is no way to query "all playthroughs belonging to player X".

The application flow diagram (`docs/Application_Flow.mmd`) shows a start menu between login and gameplay with "New Game" and "Load Game" branches. This design implements that flow.

## Goals / Non-Goals

**Goals:**

- Insert a start menu phase into the game engine's phase state machine
- Associate playthroughs with a player username so they can be queried
- Present a list of saved playthroughs when the player chooses "Load Game"
- Restore full game state from a selected playthrough and resume gameplay
- Keep the existing "New Game" flow (select adventure → character creation) intact

**Non-Goals:**

- Deleting or renaming saved playthroughs
- Multiple save slots per playthrough (auto-save continues as-is)
- Player account persistence or authentication beyond the existing login prompt
- Manual save/load commands during gameplay

## Decisions

### 1. Add `Username` to `Playthrough` and `PlaythroughDocument`

**Decision**: Store the player's username directly on the `Playthrough` entity and its MongoDB document rather than introducing a separate join table or player-to-playthrough mapping.

**Rationale**: The `Player` entity is currently ephemeral (created fresh each login with a new GUID). Introducing a persistent player record and a separate relationship table adds complexity with no benefit at this stage. A simple `Username` field on the playthrough is sufficient for querying by player and aligns with the existing flat document model.

**Alternatives considered**:
- *Persist Player entity and use a foreign key*: Adds a new collection, repository, and mapper with no current use case beyond this query. Deferred to future multiplayer work.
- *Store playthrough IDs on the Player document*: Requires persisting Player, introduces bidirectional coupling.

### 2. Add a `StartMenu` phase to `GamePhase`

**Decision**: Insert a `StartMenu` enum value between `Login` and `NewAdventure`. After login, the engine transitions to `StartMenu` instead of `NewAdventure`. The start menu presents two options: "1. New Game" and "2. Load Game".

**Rationale**: This follows the existing pattern of the `GamePhase` enum driving a switch in `GameEngine.ProcessInput`. A new phase keeps the state machine explicit and testable.

**Alternatives considered**:
- *Sub-state within Login phase*: Overloads the login phase with menu logic, harder to test independently.
- *Separate state machine class*: Over-engineering for a two-option menu.

### 3. Add a `LoadGame` phase for playthrough selection

**Decision**: When the player chooses "Load Game", transition to a new `LoadGame` phase that lists saved playthroughs and accepts a numeric selection. This mirrors how `NewAdventure` works (numeric selection from a list).

**Rationale**: Consistent UX pattern — the player is already familiar with numeric selection from adventure selection. Separating this into its own phase keeps each handler method focused.

### 4. Query playthroughs via `IPlaythroughRepository.FindByUsername`

**Decision**: Add a `FindByUsername(string username)` method to `IPlaythroughRepository` that returns a list of playthrough summaries (ID, adventure name, character name, location). The `IDataContext` interface gains a generic query method or the repository queries MongoDB directly.

**Rationale**: The current `IDataContext` only supports `Load` by ID. Rather than adding a generic query abstraction to the framework (which may be premature), the `PlaythroughRepository` can use `IMongoDatabase` directly for this query. This keeps the change localised.

**Alternatives considered**:
- *Add a generic `Query<T>(filter)` method to `IDataContext`*: More reusable but speculative — no other query use cases exist yet.
- *Filter client-side by loading all playthroughs*: Does not scale, even for Phase 0.

### 5. Resume flow bypasses character creation

**Decision**: When loading a playthrough, the engine sets the `PlaythroughId` on `GameSession`, transitions directly to `GamePhase.Playing`, and calls `StartAdventure` to display the current room. Character creation and adventure selection are skipped entirely.

**Rationale**: The loaded playthrough already contains all character and state data. The `StartAdventure` method already handles displaying the current room — it just needs to be called with the loaded playthrough.

### 6. Store username on `GameSession`

**Decision**: Extend `IGameSession` to also store the logged-in `Username` so it's available when creating a new playthrough (to set the username field) and when querying saved playthroughs.

**Rationale**: The username is currently only available within the `LoggedInResponse`. Storing it on the session makes it accessible to the engine during both the start menu and new game flows without threading it through method parameters.

## Risks / Trade-offs

- **Username is not a unique identifier** → Players can overwrite each other's saves if they use the same username. This is acceptable for Phase 0 single-player; proper authentication is a future concern.
- **MongoDB query bypasses `IDataContext` abstraction** → The `FindByUsername` query couples `PlaythroughRepository` more directly to MongoDB. Mitigation: this is already the case for the repository implementations; the abstraction leak is contained within the persistence layer.
- **Existing playthroughs in MongoDB will lack `Username`** → Any playthroughs saved before this change won't appear in "Load Game". Mitigation: this is pre-release software with no production data. No migration needed.
- **Start menu adds an extra step to the flow** → Players must now choose "New Game" instead of going directly to adventure selection. This is the expected UX per the application flow diagram.
