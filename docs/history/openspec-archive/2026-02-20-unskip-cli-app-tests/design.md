## Context

`GameEngine` was refactored to accept `GameContentLoader` instead of `GameState` as a constructor dependency, enabling `LaunchGame` to load adventure content from JSON files. The `CliAppTests` were written against the old constructor signature `GameEngine(Parser, RequestSender, GameState)` and skipped until `LaunchGame` was implemented. Now they need updating to match the new signature `GameEngine(Parser, RequestSender, GameContentLoader)`.

Additionally, `CliApp.InitiateCharacterSetup` does not handle null input from `ReadLine()`, causing tests to hang when `FakeConsole` runs out of queued input.

## Goals / Non-Goals

**Goals:**

- Make all 7 `CliAppTests` compile and pass
- Introduce `IGameContentLoader` interface for testability
- Add `FakeGameContentLoader` test double returning pre-built `WorldContent`
- Handle EOF (null input) gracefully in `CliApp.InitiateCharacterSetup`
- Sync `game-loop` spec with current startup display behaviour

**Non-Goals:**

- Changing `GameEngine.LaunchGame` logic
- Adding new test scenarios
- Refactoring `GameContentLoader` internals

## Decisions

### Extract `IGameContentLoader` interface

**Decision**: Introduce `IGameContentLoader` with a single `WorldContent Load(string adventureId)` method. `GameEngine` depends on the interface; `GameContentLoader` implements it.

**Why over alternatives**:
- Making `Load` virtual: leaks implementation detail, not idiomatic for DI-based codebase
- Passing `Func<string, WorldContent>` delegate: less discoverable, breaks DI registration pattern
- Adding test-only constructor overload to `GameEngine`: pollutes production API with test concerns

**Impact**: `ServiceCollectionExtensions` registers `GameContentLoader` as `IGameContentLoader`.

### Add `GameBuilder.BuildWorldContent(startingRoomId)`

**Decision**: Add a method to `GameBuilder` that returns `WorldContent` (rooms, empty barriers, starting room ID), reusing the existing room-building infrastructure.

**Why**: Tests currently use `GameBuilder` to construct rooms. This method bridges to the new `WorldContent` type without duplicating builder logic.

### Create `FakeGameContentLoader`

**Decision**: A simple `IGameContentLoader` implementation that takes a `WorldContent` at construction and returns it from `Load()`, ignoring the `adventureId` parameter.

**Why**: Keeps test setup explicit — tests build rooms with `GameBuilder`, wrap in `WorldContent`, pass to fake loader.

### Handle null input in `InitiateCharacterSetup`

**Decision**: Check for null after each `ReadLine()` call in `InitiateCharacterSetup`. If null, return a default `Character` or exit the application gracefully.

Looking at the test `Null_input_exits_loop` — it expects `Run()` to complete without hanging when no input is queued. The simplest approach: if `ReadLine()` returns null at the initial "Hit enter" prompt or during the creation loop, exit the method. Since `LaunchGameSession` requires a `Character`, and we can't create one without input, the method should exit the entire `Run()` flow. This means `InitiateCharacterSetup` returns `Character?` and `Run()` checks for null before proceeding.

## Risks / Trade-offs

- **New interface file**: Adds one file (`IGameContentLoader.cs`) — minimal overhead for proper testability.
- **Null return from `InitiateCharacterSetup`**: Changes the return type to `Character?`. This is the minimal change to handle EOF — the alternative (throwing) would require try/catch in `Run()` which is heavier.
