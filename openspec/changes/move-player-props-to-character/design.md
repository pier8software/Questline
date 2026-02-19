## Context

The `character-creation` spec defines Player and Character as separate models. Currently, `Player` directly owns `Location` (string) and `Inventory` (Inventory). These are properties of the in-game avatar, not the human player. The Character bounded context folder (`Domain/Characters/Entity/`) exists but is empty. This change introduces the `Character` class and moves `Location` and `Inventory` from `Player` to `Character`, aligning the domain model with the spec's Player/Character separation before the full character-creation feature is implemented.

Seven handlers and the `GameBuilder` test helper reference `state.Player.Location` or `state.Player.Inventory`.

## Goals / Non-Goals

**Goals:**
- Introduce `Character` class in `Domain/Characters/Entity/` with `Location` and `Inventory`
- Update `Player` to hold a `Character` reference instead of `Location` and `Inventory` directly
- Update all handlers and test helpers to route through `Player.Character`
- Keep all existing tests passing with updated property paths

**Non-Goals:**
- Adding character-creation fields (Name, Race, Class, Stats) — that is the full `character-creation` feature
- Changing command pipeline, parser, or game loop
- Adding new commands or responses
- Modifying JSON content or serialisation

## Decisions

### 1. Character as a mutable class (not a record)

**Decision:** `Character` will be a class with `required` properties and a mutable `Location` setter, matching the current `Player` pattern.

**Rationale:** `Location` must be mutable (handlers write `state.Player.Location = ...`). Using a class keeps consistency with the existing `Player` pattern. When the full character-creation feature lands, it will extend this class with Name, Race, Class, Stats, etc.

**Alternative considered:** Record with `with` expressions — rejected because it would require propagating new instances back through `Player` and `GameState`, adding complexity for no benefit.

### 2. Player delegates to Character

**Decision:** `Player` gets a `required Character Character { get; init; }` property. Remove `Location` and `Inventory` from `Player`.

**Rationale:** Direct delegation is the simplest path. All existing code changes from `state.Player.Location` to `state.Player.Character.Location` — a mechanical find-and-replace.

**Alternative considered:** Keeping facade properties on `Player` (e.g., `public string Location => Character.Location`) — rejected as it hides the real ownership and adds maintenance burden.

### 3. Character lives in the Characters bounded context

**Decision:** `Domain/Characters/Entity/Character.cs` in namespace `Questline.Domain.Characters.Entity`.

**Rationale:** The `Characters/Entity/` folder already exists. This follows the domain feature-folder convention.

## Risks / Trade-offs

- **Widespread mechanical changes** → Low risk since all changes follow the same pattern (`Player.X` → `Player.Character.X`). Existing tests validate correctness.
- **Two-step character-creation** → The `Character` class is introduced without Name/Race/Class/Stats. This means a second change will extend it. Trade-off accepted: shipping the structural refactor separately keeps PRs focused and reviewable.
