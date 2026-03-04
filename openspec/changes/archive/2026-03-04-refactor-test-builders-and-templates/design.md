## Context

Test data construction across the Questline test suite is inconsistent. Items, barriers, features, and rooms are built inline with object initialisers, duplicated across 8+ test files. The testing skill prescribes a Builders + Templates pattern using `TestStack.Dossier`, but this hasn't been implemented yet.

Current state:
- `RoomBuilder` — hand-rolled primary-constructor builder that takes `(id, name, description)` and returns `Room`
- `GameBuilder` — orchestration builder that composes rooms, inventory, and unlocked barriers into a `GameFixture`
- No builders exist for `Item`, `Barrier`, `Feature`, `Exit`, `Adventure`, or `Playthrough`
- No Template classes exist

Recurring inline test data: "brass lamp" item (5 files), "iron-door" barrier (3 files), "cellar"/"chamber" rooms (6+ files), "rusty key" item (3 files).

## Goals / Non-Goals

**Goals:**
- Introduce `TestStack.Dossier`-based builders for all domain entities used in tests
- Create Template classes with pre-configured builders for commonly reused test objects
- Refactor `GameBuilder` to accept `RoomBuilder` instances directly (not just string params)
- Update all test files to use Templates where inline initialisers are duplicated
- All existing tests continue to pass with identical assertions

**Non-Goals:**
- Changing production code or domain entity definitions
- Adding, removing, or modifying any test assertions
- Creating builders for entities not used in tests (e.g. `Character`)
- Building a `ScenarioBuilder` (keep `GameBuilder` as the top-level orchestrator for now)

## Decisions

### 1. Use `TestStack.Dossier` for builder base class

**Choice**: Extend `TestDataBuilder<T, TBuilder>` from Dossier for all entity builders.

**Rationale**: The testing skill explicitly prescribes this library. Dossier provides `Set(x => x.Property, value)` fluent API and `Build()` method, reducing boilerplate. The pattern is consistent and well-understood.

**Alternative considered**: Continue with hand-rolled builders. Rejected because it leads to inconsistent APIs and more code to maintain.

### 2. Builder per domain entity, Templates as static builder factories

**Choice**: One builder class per entity (`ItemBuilder`, `RoomBuilder`, `BarrierBuilder`, `FeatureBuilder`, `ExitBuilder`, `AdventureBuilder`, `PlaythroughBuilder`). Template classes (`Items`, `Rooms`, `Barriers`, `Features`) return pre-configured *builder instances* (not built objects), so callers can customise before building.

**Rationale**: Returning builders from Templates allows tests to override specific properties (e.g. `Templates.Rooms.Cellar.WithItems([lamp])`). This matches the skill examples.

### 3. Refactor `GameBuilder.WithRoom` to accept `RoomBuilder`

**Choice**: Add `WithRoom(RoomBuilder builder)` overload that calls `builder.Build()` internally. Keep `GameBuilder` as the top-level orchestrator.

**Rationale**: Tests read more naturally: `.WithRoom(Templates.Rooms.Cellar)` instead of `.WithRoom("cellar", "Cellar", "A damp cellar.")`. The `GameBuilder` remains responsible for wiring rooms into the fixture.

### 4. File layout

**Choice**:
- `TestHelpers/Builders/` — builder classes (one per file): `ItemBuilder.cs`, `RoomBuilder.cs`, `BarrierBuilder.cs`, `FeatureBuilder.cs`, `ExitBuilder.cs`, `AdventureBuilder.cs`, `PlaythroughBuilder.cs`
- `TestHelpers/Builders/Templates/` — template classes: `Items.cs`, `Rooms.cs`, `Barriers.cs`, `Features.cs`
- `TestHelpers/Builders/GameBuilder.cs` — updated orchestrator (fakes stay in this file)

**Rationale**: Follows the structure shown in the testing skill (`TestHelpers/Builders/Templates/`). One file per builder and one file per template class keeps things discoverable.

### 5. `RoomBuilder` handles exits and items via Dossier `Set`

**Choice**: `RoomBuilder` will use Dossier's `Set` for simple properties (Id, Name, Description) and maintain internal collections for Exits, Items, Features (since these are additive). The `Build()` override will merge both.

**Rationale**: Dossier's `Set` works well for scalar properties. For collection-based properties that need additive `With*` methods (e.g. `WithExit`, `WithItem`), internal lists are more ergonomic.

## Risks / Trade-offs

- **[New dependency]** `TestStack.Dossier` adds a NuGet dependency to the test project only. → Mitigation: test-only dependency, no production impact. Library is stable and lightweight.
- **[Large changeset]** Touching 8+ test files in one change. → Mitigation: assertions don't change, only Arrange sections. Run full test suite to verify.
- **[Learning curve]** Team needs to know the builder/template pattern. → Mitigation: the testing skill documents the pattern clearly with examples.
