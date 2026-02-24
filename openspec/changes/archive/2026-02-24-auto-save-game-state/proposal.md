## Why

The existing `save-and-load` spec bundles saving and loading into a single feature with a manual `save` command and file-based persistence. Splitting these concerns and switching to automatic persistence via MongoDB sets the foundation for multiplayer persistence and eliminates the risk of players losing progress by forgetting to save.

## What Changes

- **Auto-save via decorator**: Game state is automatically persisted to MongoDB after every request handler completes, transparent to both the handler and the player
- **MongoDB persistence**: Replace the planned file-based JSON save with a MongoDB-backed repository using upsert semantics
- **Single `IGameStateRepository`**: A repository interface with a `Save(GameState)` method that upserts the full game state document; a single implementation backed by MongoDB for the POC
- **`AutoSaveDecorator<TRequest>`**: A decorator wrapping `IRequestHandler<TRequest>` that calls `Save()` after the inner handler returns, applied to all handlers unconditionally
- **Startup flow reorder**: `CliApp` changes from character-creation-then-launch to content-load-and-save-then-character-creation-and-save-then-game-loop
- **Full snapshot on game start**: When a new game begins, the adventure content is loaded from JSON and the complete world state is saved to MongoDB as the initial profile; character data is saved into this profile after creation
- **No `save` command**: Persistence is invisible to the player
- **Supersedes save portion of `save-and-load`**: The load portion will be addressed in a separate future change

## Non-goals

- **Load/resume game**: Restoring a saved game is a separate future spec
- **Multiple save profiles**: A single active profile per player is sufficient for the POC
- **`save` command**: Auto-save replaces explicit player-initiated saving
- **Multiple repositories**: A single `IGameStateRepository` covers the POC; splitting into per-aggregate repositories (e.g., `ICharacterRepository`, `IRoomRepository`) is deferred

## Capabilities

### New Capabilities

- `auto-save`: Automatic game state persistence to MongoDB after every player action via a handler decorator and a game state repository

### Modified Capabilities

- `game-loop`: Startup sequence changes — adventure content is loaded and saved to the database before character creation, and the character is saved to the profile after creation, before the game loop begins

## Impact

- **New dependency**: MongoDB C# driver (`MongoDB.Driver` NuGet package)
- **Infrastructure**: Requires a running MongoDB instance (local or containerised)
- **`Framework/`**: New `IGameStateRepository` interface and MongoDB implementation
- **`Engine/`**: New `AutoSaveDecorator<TRequest>`, DI registration changes to wrap handlers
- **`Cli/CliApp`**: Startup flow reordered — content load and profile save before character creation
- **`Domain/`**: No changes — entities remain persistence-unaware
- **Existing `save-and-load` spec**: Save requirements superseded by this change; load requirements deferred to a future change
