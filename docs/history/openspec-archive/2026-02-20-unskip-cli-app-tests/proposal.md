## Why

The `CliAppTests` were skipped pending implementation of `GameEngine.LaunchGame`. Now that `LaunchGame` is implemented, the tests need to be unskipped and updated to work with the new `GameEngine` constructor signature, which takes `GameContentLoader` instead of `GameState`. Additionally, null input handling in `CliApp.InitiateCharacterSetup` needs to be addressed, and the `game-loop` spec is out of sync with the current implementation.

## What Changes

- Remove `Skip` attributes from all 7 `CliAppTests`
- Update `CliAppTests.CreateCliApp()` to construct `GameEngine` with a test `IGameContentLoader` instead of `GameState`
- Extract `IGameContentLoader` interface so `GameEngine` can be tested with fake content
- Add `BuildWorldContent(startingRoomId)` to `GameBuilder` for test convenience
- Handle null input in `CliApp.InitiateCharacterSetup` to exit gracefully on EOF
- Update `game-loop` spec to reflect current startup display (items, locked barriers)

## Non-goals

- Changing the `GameEngine.LaunchGame` implementation
- Adding new test coverage beyond fixing existing skipped tests
- Modifying the character creation flow

## Capabilities

### New Capabilities

_None — this change fixes existing tests and syncs specs._

### Modified Capabilities

- `game-loop`: Update startup display requirement to include items and locked barriers, and document EOF handling during character setup

## Impact

- `src/Questline/Engine/Core/GameEngine.cs` — depend on `IGameContentLoader` interface
- `src/Questline/Engine/Content/GameContentLoader.cs` — implement new interface
- `src/Questline/Engine/Content/IGameContentLoader.cs` — new interface file
- `src/Questline/Cli/CliApp.cs` — null input handling in `InitiateCharacterSetup`
- `src/Questline/Cli/ServiceCollectionExtensions.cs` (or DI registration) — register interface
- `tests/Questline.Tests/Cli/CliAppTests.cs` — unskip and fix all tests
- `tests/Questline.Tests/TestHelpers/Builders/GameBuilder.cs` — add `BuildWorldContent`
- `tests/Questline.Tests/TestHelpers/FakeGameContentLoader.cs` — new test double
- `openspec/specs/game-loop/spec.md` — sync with implementation
