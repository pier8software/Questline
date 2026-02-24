## Context

Domain entities (`Character`, `Room`, `Barrier`, `Inventory`) currently use a mix of mutable properties and in-place collection mutation. Handlers in the Engine layer reach into entities and modify them directly—`room.Items.Remove(item)`, `barrier.IsUnlocked = true`, `character.SetLocation(...)`. This works today but couples mutation logic to the Engine, makes state transitions implicit, and prevents future capabilities like undo/redo or event sourcing.

The codebase already leans toward immutability: `Character` is a `record`, value objects (`HitPoints`, `AbilityScores`, `Exit`) are records, and `Player` is an immutable record wrapper. The goal is to complete this pattern across all domain entities.

## Goals / Non-Goals

**Goals:**
- All domain entities are immutable after construction—no mutable properties, no mutable collections.
- State changes are expressed as methods on the entity that return a new instance (e.g., `character.MoveTo("room-2")` returns a new `Character`).
- `GameState` provides update methods so handlers can swap in new entity instances.
- All existing tests continue to pass with updated assertions.

**Non-Goals:**
- Event sourcing, domain events, or change tracking (future work).
- Changing command pipeline contracts (`IRequest`, `IResponse`, `IRequestHandler` signatures remain as-is).
- Modifying JSON content loading or `Data/` DTOs.
- Optimising memory allocation—correctness over performance at Phase 0 scale.

## Decisions

### 1. Use C# records with `with` expressions for entity immutability

**Decision:** Convert `Room`, `Barrier`, and `Inventory` to records. `Character` and `Player` are already records.

**Rationale:** Records provide value equality, built-in `with` expression support, and align with the project's existing convention ("prefer records for immutable data"). The `with` expression creates shallow copies with modified properties, which is efficient for small object graphs.

**Alternative considered:** Keeping classes with private setters and returning `this` from mutation methods. Rejected because it doesn't enforce immutability at the type level—callers could still cast and mutate.

### 2. Immutable collections via `ImmutableList<T>` for `Inventory`, `Room.Items`, `Room.Exits`, `Room.Features`

**Decision:** Replace `List<Item>`, `Dictionary<Direction, Exit>`, and `List<Feature>` with `ImmutableList<Item>`, `ImmutableDictionary<Direction, Exit>`, and `ImmutableList<Feature>` from `System.Collections.Immutable`.

**Rationale:** Immutable collections prevent accidental mutation and integrate cleanly with the `with` pattern. They provide structural sharing so copies are efficient. `System.Collections.Immutable` is a first-party .NET library, no new dependency.

**Alternative considered:** Using `IReadOnlyList<T>` backed by regular lists. Rejected because `IReadOnlyList` is only a read-only view—the backing list can still be mutated if referenced elsewhere.

### 3. Domain entity mutation methods return new instances

**Decision:** Each entity gets domain-specific methods that return a new instance:

- `Character.MoveTo(string locationId)` → new `Character`
- `Character.AddInventoryItem(Item item)` → new `Character`
- `Character.RemoveInventoryItem(Item item)` → new `Character`
- `Room.AddItem(Item item)` → new `Room`
- `Room.RemoveItem(Item item)` → new `Room`
- `Barrier.Unlock()` → new `Barrier`
- `Inventory.Add(Item item)` → new `Inventory`
- `Inventory.Remove(Item item)` → new `Inventory`

**Rationale:** Domain methods express intent clearly (`MoveTo` vs setting a property). Returning new instances enforces immutability at the call site—callers must use the returned value.

### 4. `GameState` gets replace/update methods

**Decision:** Add methods to `GameState` for replacing entities:

- `UpdatePlayer(Player player)` — replaces the current player
- `UpdateRoom(Room room)` — replaces a room by its `Id`
- `UpdateBarrier(Barrier barrier)` — replaces a barrier by its `Id`

`GameState` itself remains a mutable container (it is a `Data` object, not a domain entity). The dictionaries inside it are mutable so entities can be swapped in. This is intentional—`GameState` is the one place where "the world changes" is coordinated.

**Rationale:** Handlers need a single place to commit entity replacements. Keeping `GameState` mutable avoids cascading immutability up through the handler signatures, which would complicate the mediator pattern.

**Alternative considered:** Making `GameState` immutable too (handlers return new `GameState`). Rejected because it would require changing the `IRequestHandler.Handle` signature from `IResponse` to `(IResponse, GameState)`, affecting every handler and the mediator framework. This is a worthwhile future change but out of scope.

### 5. `Player` updated via record `with` expression

**Decision:** Since `Player` is already `record Player(string Id, Character Character)`, updating the character is simply `state.UpdatePlayer(state.Player with { Character = newCharacter })`.

**Rationale:** No new methods needed on `Player`—the record `with` syntax is sufficient and idiomatic.

## Risks / Trade-offs

- **[Allocation increase]** → Every state change creates new objects. Mitigated: Phase 0 has tiny object graphs (5 rooms, few items). Immutable collections use structural sharing. Not a concern until multiplayer scale.
- **[Handler verbosity]** → Handlers need to capture return values and call `state.Update*()` instead of one-line mutations. Mitigated: This is intentional—explicit is better than implicit. The extra lines make state transitions visible.
- **[Serialisation compatibility]** → `System.Text.Json` needs to deserialise into immutable records. Mitigated: Records with `init` properties work with `System.Text.Json` out of the box. Content loading constructs entities via builders/factories, not direct deserialisation into domain types.
- **[Test builder changes]** → `GameBuilder` and `RoomBuilder` need updates to construct immutable entities. Mitigated: Builders already use a construction pattern that maps cleanly to record initialisation.
