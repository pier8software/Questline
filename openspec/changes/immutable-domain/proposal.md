## Why

Domain entities (`Character`, `Room`, `Barrier`, `Inventory`) currently expose mutable properties and collections that handlers modify directly (e.g., `room.Items.Remove(item)`, `barrier.IsUnlocked = true`). This scatters mutation logic across the Engine layer, making it hard to reason about state transitions and opening the door to accidental corruption. Making domain entities fully immutable—where all state changes go through explicit methods on the entity itself—centralises business rules, improves testability, and prepares the domain for future event sourcing or undo/redo capabilities.

## What Changes

- **Immutable `Character`**: Replace `SetLocation()` mutation and direct `Inventory` access with methods (`MoveTo`, `AddInventoryItem`, `RemoveInventoryItem`) that return new `Character` instances using `with` expressions.
- **Immutable `Room`**: Convert to a record; replace mutable `Items`/`Features` collections with methods (`AddItem`, `RemoveItem`) that return new `Room` instances.
- **Immutable `Barrier`**: Convert to a record; replace mutable `IsUnlocked` setter with an `Unlock()` method that returns a new `Barrier`.
- **Immutable `Inventory`**: Replace `Add`/`Remove` mutation with methods returning new `Inventory` instances backed by immutable collections.
- **`GameState` update API**: Add methods to replace entities by key (e.g., `UpdateRoom`, `UpdateBarrier`, `UpdatePlayer`) so handlers can swap in new immutable instances.
- **Handler refactoring**: Update all handlers (`MovePlayerCommandHandler`, `TakeItemHandler`, `DropItemCommandHandler`, `UseItemCommandHandler`) to use the new immutable APIs.
- **Test updates**: Update all domain and handler tests plus `GameBuilder`/`RoomBuilder` to work with immutable entities.

## Non-goals

- Introducing event sourcing, domain events, or an undo/redo system (future work that benefits from this change).
- Changing the public command pipeline or response contracts—external behaviour remains identical.
- Modifying content loading or JSON serialisation; `Data/` DTOs are unaffected.

## Capabilities

### New Capabilities

- `immutable-domain`: Architectural constraint requiring all domain entities to be immutable, with state changes expressed as methods returning new instances.

### Modified Capabilities

_(No spec-level behaviour changes—this is a pure implementation refactoring. All existing command behaviour, responses, and game rules remain identical.)_

## Impact

- **Domain layer** (`src/Questline/Domain/`): Every entity and `Inventory` gains new immutable API; mutable setters/collections removed.
- **Engine layer** (`src/Questline/Engine/Handlers/`): All four mutation-performing handlers refactored to compose new entity instances and update `GameState`.
- **Shared** (`GameState`): New replace/update methods added.
- **Tests** (`tests/Questline.Tests/`): Domain tests, handler tests, and test builders updated to match new immutable patterns.
- **No API/dependency/CI changes**—purely internal refactoring.
