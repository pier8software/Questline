## Context

Room exits are currently `Dictionary<Direction, string>` — a direction mapped to a destination room ID. The JSON content layer already supports richer exits via `ExitDto` (with an optional `Barrier` field and a custom `ExitDictionaryConverter`), but the domain model discards this information during content loading (`BuildRooms` only maps `exit.Destination`).

Phase 0.4 (puzzles-and-barriers) needs exits to carry barrier references. This refactor introduces an `Exit` value object in the domain so the structural change is isolated from feature work.

## Goals / Non-Goals

**Goals:**

- Introduce `Exit` as a domain value object with `Destination` and optional `BarrierId`
- Update `Room.Exits` to `Dictionary<Direction, Exit>`
- Preserve the ergonomic `WorldBuilder` API for the common case (no barrier)
- Thread `ExitDto.Barrier` through to `Exit.BarrierId` in the content loader
- All existing tests pass with equivalent behaviour — no user-facing change

**Non-Goals:**

- Barrier entity, barrier state, or barrier logic (deferred to puzzles-and-barriers)
- Barrier validation in content loading (no barriers.json exists yet)
- Changes to the JSON content format (already supports barriers)

## Decisions

### Decision 1: Exit as a record (value object)

`Exit` will be a C# record in `Questline.Domain`:

```csharp
public record Exit(string Destination, string? BarrierId = null);
```

**Why a record?** Exits are value objects — they have no identity, are compared by value, and are immutable. Records are the project's convention for value objects (per `config.yaml`). The optional `BarrierId` parameter keeps construction clean for the common case: `new Exit("hallway")`.

**Alternative considered:** A class with `required` properties (matching the entity pattern). Rejected because exits have no identity — they don't need `Id` and should use structural equality.

### Decision 2: WorldBuilder keeps simple overload

`RoomBuilder.WithExit` keeps its current `(Direction, string)` signature as a convenience that wraps to `new Exit(destinationId)`. A second overload `(Direction, Exit)` allows passing a full `Exit` when needed.

```csharp
public RoomBuilder WithExit(Direction direction, string destinationId)
    => WithExit(direction, new Exit(destinationId));

public RoomBuilder WithExit(Direction direction, Exit exit) { ... }
```

**Why?** Most tests and content don't use barriers. Forcing `new Exit(...)` everywhere would add noise for zero benefit. The string overload is sugar that delegates to the real one.

### Decision 3: Handlers use Exit.Destination

`GoCommandHandler` changes from:
```csharp
currentRoom.Exits.TryGetValue(command.Direction, out var destinationId)
```
to:
```csharp
currentRoom.Exits.TryGetValue(command.Direction, out var exit)
// then use exit.Destination
```

`LookCommandHandler` continues to read `room.Exits.Keys` for direction names — unchanged.

### Decision 4: Content loader maps BarrierId through

`FileSystemAdventureLoader.BuildRooms` currently creates `Dictionary<Direction, string>`. It will change to create `Dictionary<Direction, Exit>`, mapping `ExitDto.Barrier` → `Exit.BarrierId`. The barrier field will flow through even though nothing consumes it yet.

## Risks / Trade-offs

- **Wide touchpoint for a small change** — Every test that uses `WorldBuilder.WithExit` or reads `Room.Exits` will need updating. Mitigated by the string overload on `WithExit` which means most test setup code stays identical; only assertions on exit values change.
- **BarrierId references nothing yet** — `Exit.BarrierId` can hold a string that doesn't resolve to any barrier entity. This is intentional — validation belongs to the puzzles-and-barriers change when barriers actually exist.
