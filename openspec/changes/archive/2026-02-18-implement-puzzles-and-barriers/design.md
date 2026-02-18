## Context

Phase 0.4 adds puzzle mechanics: the `use` command, barrier system, `examine` command, and room features. The `Exit` record already has an optional `BarrierId` field (from the refactor-exit-model change). The adventure JSON already references `"barrier": "iron-door"` on exits. This change implements the full barrier lifecycle and the new commands.

## Goals / Non-Goals

**Goals:**

- Barrier entity with lock/unlock state and item-based unlocking
- Feature entity for examinable room objects with keyword matching
- `use <item> [on <target>]` command with targeted and contextual modes
- `examine <target>` command searching inventory items, room items, then features
- Movement blocked by locked barriers, allowed when unlocked
- Look and move responses display locked barrier descriptions
- Adventure JSON content with barriers array and room features
- GameContentLoader builds barriers and features from JSON

**Non-Goals:**

- Consuming items on use (key stays in inventory)
- Multiple barrier types (only item-unlock)
- Barrier validation in content loading (no cross-reference validation)

## Decisions

### Decision 1: Barrier as a mutable entity in Domain/Rooms/Entity

`Barrier` is a class (not a record) because it has identity (`Id`) and mutable state (`IsUnlocked`). It lives in `Domain/Rooms/Entity/` alongside `Room`, `Exit`, and `Feature` since barriers are conceptually tied to room exits.

```csharp
public class Barrier
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string BlockedMessage { get; init; }
    public required string UnlockItemId { get; init; }
    public required string UnlockMessage { get; init; }
    public bool IsUnlocked { get; set; }
}
```

**Why mutable?** Barrier state changes during gameplay. Unlike rooms/exits (which are structurally immutable), barriers need runtime mutation when unlocked.

### Decision 2: Barriers stored in GameState, not in rooms

Barriers live in a `Dictionary<string, Barrier>` on `GameState`, not embedded in individual rooms. Multiple exits across different rooms could reference the same barrier by ID. Centralising barriers in GameState makes this possible and keeps the lookup simple via `GetBarrier(string? id)`.

### Decision 3: Feature keyword matching for examine

Features support both name matching and keyword matching. The examine handler searches in order: inventory items → room items → room features. Feature matching is case-insensitive on both `Name` and `Keywords` entries, so `examine symbols` matches a feature named "strange symbols" if "symbols" is in its keywords.

### Decision 4: Use command "on" keyword splitting

`use rusty key on iron door` splits on `" on "` to separate item name from target name. `use rusty key` (no "on") does contextual use — finds the first locked barrier in the room. This avoids complex natural language parsing while being intuitive.

### Decision 5: Locked barrier display in room descriptions

Room descriptions include locked barrier lines between items and exits:
```
Room Name
Description
You can see: item1, item2
A locked iron door blocks the way North.
Exits: North, South
```

Both `RoomDetailsResponse` and `PlayerMovedResponse` gain an optional `lockedBarriers` parameter (defaults to null, coalesced). Existing callers are unaffected.

## Risks / Trade-offs

- **`System.Threading.Barrier` name conflict**: The `Barrier` class in `Questline.Domain.Rooms.Entity` conflicts with `System.Threading.Barrier` via implicit global usings. Files outside the `Questline.Domain.Rooms.Entity` namespace that reference `Barrier` need `using Barrier = Questline.Domain.Rooms.Entity.Barrier;`.
- **Contextual use finds first locked barrier**: If a room has multiple locked barriers, contextual use picks the first one found. This is acceptable for Phase 0 where rooms have at most one barrier.
