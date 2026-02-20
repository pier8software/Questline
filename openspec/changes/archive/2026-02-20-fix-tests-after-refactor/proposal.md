## Why

The character creation system was refactored — `Character` and `Player` are now records, `CharacterStats` was split into `HitPoints` and `AbilityScores` value objects, `CharacterFactory` was replaced by a `CharacterCreationStateMachine`, and `CharacterNameValidator` now uses FluentValidation. The test project (230 build errors) is broken because test code still references the old constructors, types, and APIs. The `character-creation` spec's implementation notes are out of date.

## What Changes

- Fix all test files to use new `Character.Create()` factory method, `Player(id, character)` record constructor, and `SetLocation()` method
- Update `GameBuilder` test helper to construct `Character` and `Player` with new APIs
- Rewrite `CharacterCreationServiceTests` to test `CharacterCreationStateMachine`
- Delete or rewrite `StatsQueryHandlerTests` (handler was removed)
- Update `CliAppTests` for the new `CliApp` constructor signature
- Update the `character-creation` spec implementation notes to reflect new record types, value objects, and state machine pattern

## Capabilities

### New Capabilities

_None._

### Modified Capabilities

- `character-creation`: Implementation notes must reflect the new `Character` record with `Character.Create()` factory, `HitPoints`/`AbilityScores` value objects, `Player` record, `CharacterCreationStateMachine`, and FluentValidation-based `CharacterNameValidator`

## Non-goals

- Adding new game features or commands
- Changing any game behaviour — this is a test-fix and spec-sync change only
- Re-implementing the deleted `StatsQueryHandler` (that will be addressed in a future change)

## Impact

- **Tests**: All test files using `Character`, `Player`, or `GameBuilder` need constructor/API updates
- **Specs**: `character-creation` spec implementation notes need updating
- **No runtime code changes** — the production code refactoring is already complete
