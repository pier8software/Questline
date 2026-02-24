## Context

Game state currently lives only in memory (`GameEngine._state`). If the process exits, all progress is lost. The project needs persistence as a foundation for save/load and eventually multiplayer. The domain entities follow an immutable pattern — mutation methods return new instances and `GameState` provides replacement methods — meaning the state snapshot after a handler completes is always consistent and safe to persist.

## Goals / Non-Goals

**Goals:**

- Persist game state to MongoDB automatically after every request handler completes
- Keep handlers and domain entities completely unaware of persistence
- Establish a repository pattern that can evolve toward per-aggregate repositories later
- Save the full game state as a single MongoDB document per player session

**Non-Goals:**

- Load/resume from a saved game (separate future spec)
- Multiple save profiles or save slots
- Splitting into multiple repositories (`ICharacterRepository`, `IRoomRepository`)
- Adding a `save` command or any player-facing persistence UI
- Optimistic concurrency or conflict resolution

## Decisions

### 1. Auto-save via handler decorator

**Choice**: `AutoSaveDecorator<TRequest>` wrapping `IRequestHandler<TRequest>`

**Alternatives considered**:
- **(A) Each handler calls the repo explicitly** — simple but repetitive, couples every handler to persistence
- **(C) Save inside `RequestSender.Send()`** — single point of change but mixes dispatch with persistence concerns

**Rationale**: The decorator keeps persistence as a cross-cutting concern. Handlers stay focused on domain logic. The decorator is registered via DI so it's transparent — handlers don't know they're being wrapped. The decorator calls `Save()` unconditionally after every request (commands and queries alike), relying on MongoDB's change tracking to skip no-op writes.

### 2. Single `IGameStateRepository` with `Save(GameState)` upsert

**Choice**: One repository interface with a single `Save` method that upserts the full game state document.

**Alternatives considered**:
- **Separate `Create` and `Update` methods** — unnecessary since MongoDB upsert handles both cases
- **Multiple per-aggregate repositories** — cleaner separation but over-engineered for the POC

**Rationale**: A single method simplifies the interface. The first call inserts the document; subsequent calls update it. MongoDB's upsert semantics make this natural. The interface lives in `Framework/Persistence/`.

### 3. MongoDB document model with DTOs

**Choice**: A `GameStateDocument` DTO class in `Framework/Persistence/` that maps to/from the domain `GameState`.

**Alternatives considered**:
- **Map domain entities directly via BsonClassMap** — leaks MongoDB concerns into the domain; violates the architecture rule that Domain has no external dependencies
- **Use `System.Text.Json` serialisation to BSON** — adds unnecessary complexity

**Rationale**: The domain must stay persistence-unaware. A dedicated document class provides a clean mapping boundary. The document is a full snapshot of the game state:

```
GameStateDocument
├── Id (player session ID)
├── AdventureId
├── Version
├── Player
│   ├── Id
│   └── Character
│       ├── Name, Race, Class, Level, Experience
│       ├── AbilityScores
│       ├── HitPoints
│       ├── Location
│       └── Inventory [Items]
├── Rooms [{Id, Name, Description, Exits, Items, Features}]
└── Barriers [{Id, Name, ..., IsUnlocked}]
```

### 4. DI registration with a helper method

**Choice**: A helper method in `ServiceCollectionExtensions` that registers the concrete handler and wraps it with the decorator.

```
RegisterHandler<TRequest, THandler>(services)
  → registers THandler as concrete
  → registers IRequestHandler<TRequest> as AutoSaveDecorator<TRequest>
    wrapping THandler with IGameStateRepository
```

**Alternatives considered**:
- **Scrutor library** — adds a dependency for a single use case
- **Manual registration per handler** — verbose but no new dependencies

**Rationale**: A generic helper method keeps registration concise without introducing a new dependency.

### 5. Startup flow reorder in CliApp

**Choice**: Reorder `CliApp.Run()` to: load content → save initial world snapshot → character creation → save character → game loop.

**Current flow**: welcome → character creation → `GameEngine.LaunchGame()` (loads content + builds state)

**New flow**: welcome → load content + save initial profile → character creation + save character to profile → enter game loop

This means `GameEngine.LaunchGame()` needs refactoring — content loading moves earlier so the world state can be persisted before character creation begins.

### 6. Folder structure

```
Framework/
├── Persistence/
│   ├── IGameStateRepository.cs      ← interface
│   ├── MongoGameStateRepository.cs  ← MongoDB implementation
│   └── GameStateDocument.cs         ← document DTO
├── Mediator/
│   ├── AutoSaveDecorator.cs         ← new
│   ├── IRequestHandler.cs
│   ├── IRequest.cs
│   └── RequestSender.cs
```

## Risks / Trade-offs

**MongoDB dependency for dev/test** → Use a local MongoDB instance or Docker container. Test the repository integration separately; unit tests for handlers remain fast by mocking `IGameStateRepository`.

**DTO mapping boilerplate** → The mapping between domain entities and `GameStateDocument` is straightforward but manual. Acceptable for the POC; could introduce a mapping library later if the model grows significantly.

**Saving after every request (including queries)** → Slightly more writes than necessary but keeps the decorator simple. MongoDB's change tracking skips no-op updates, so the actual I/O overhead is negligible.

**GameEngine refactoring** → Splitting content loading from game initialisation changes the responsibilities of `GameEngine.LaunchGame()`. This is a structural improvement but touches a central piece of the codebase.

## Open Questions

- **Session ID strategy**: What identifies a save profile document? The current `Player.Id` is a `Guid.NewGuid()` generated at launch. Is this sufficient, or should the profile ID be managed separately?
- **MongoDB connection string**: Hardcoded for the POC, or configurable via `appsettings.json` / environment variable?
