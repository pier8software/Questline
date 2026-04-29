## Context

The production code has been refactored to use records, value objects, and a state machine for character creation. The test project has 230 build errors because test code still references old constructors, deleted types, and removed handlers. No runtime behaviour has changed — only the internal APIs that tests call.

Key API changes in production code:
- `Character` is now a record with `Character.Create(name, race, class, hitPoints, abilityScores, location)` factory method
- `CharacterStats` is deleted, replaced by `HitPoints(MaxHitPoints, CurrentHitPoints)` and `AbilityScores(Strength, Intelligence, Wisdom, Dexterity, Constitution, Charisma)` where each score is `AbilityScore(int Score)`
- `Player` is now `record Player(string Id, Character Character)` — positional primary constructor
- `Character.Location` is `private set` — mutated via `SetLocation(string)`
- `CharacterFactory` is deleted, replaced by `CharacterCreationStateMachine`
- `StatsQueryHandler` and `StatsQuery` are deleted
- `CharacterNameValidator` now extends `AbstractValidator<CharacterName>` (FluentValidation)
- `CliApp` constructor takes `CharacterCreationStateMachine` and `GameEngine` instead of individual dependencies

## Goals / Non-Goals

**Goals:**
- Restore the test project to a green build (zero errors, all tests pass)
- Update `GameBuilder` so all handler tests cascade-fix with minimal per-file edits
- Rewrite character creation tests to cover the new state machine
- Update `character-creation` spec implementation notes to match new types

**Non-Goals:**
- Adding new test coverage beyond what existed before
- Re-implementing the deleted `StatsQueryHandler`
- Changing any production code

## Decisions

### 1. Fix GameBuilder first to cascade-fix handler tests

**Decision**: Update `GameBuilder.DefaultCharacterFactory` and `BuildState()` to use new APIs. This fixes the majority of handler test files without touching them individually.

**Rationale**: `GameBuilder` is used by ~10 test files. Fixing it at the source reduces total file edits from ~15 to ~5-6.

### 2. Create test-only helper for `Character` construction

**Decision**: Add a static helper method in `GameBuilder` (or alongside it) that creates a default test character with sensible defaults for `HitPoints` and `AbilityScores`, so tests don't need to repeatedly construct value objects.

**Rationale**: Production `Character.Create()` requires 5+ arguments. Tests that don't care about stats shouldn't have to specify them every time.

### 3. Delete `StatsQueryHandlerTests`

**Decision**: Remove the test file since `StatsQueryHandler` and `StatsQuery` were intentionally deleted.

**Rationale**: There is no handler to test. A future change will re-introduce stats display if needed.

### 4. Rewrite `CharacterCreationServiceTests` for the state machine

**Decision**: Rename/rewrite to test `CharacterCreationStateMachine` transitions and output.

**Rationale**: The old factory tests are not salvageable — the creation flow is now multi-step with state transitions rather than a single method call.

### 5. Update `CliAppTests` for new constructor

**Decision**: Update test setup to construct `CliApp` with `CharacterCreationStateMachine` and `GameEngine` dependencies.

**Rationale**: Constructor signature changed; tests must match.

## Risks / Trade-offs

- **[Risk] Test helper defaults mask bugs** → Mitigation: Use realistic but fixed values (e.g. `HitPoints(8, 8)`, all ability scores at 10) so assertions remain meaningful
- **[Risk] Deleted StatsQueryHandlerTests reduces coverage** → Mitigation: Acceptable — the handler no longer exists; stats display will be re-tested when re-introduced
