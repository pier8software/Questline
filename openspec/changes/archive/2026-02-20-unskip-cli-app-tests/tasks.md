## 1. Extract IGameContentLoader Interface

- [x] 1.1 Create `IGameContentLoader` interface in `src/Questline/Engine/Content/IGameContentLoader.cs` with `WorldContent Load(string adventureId)` method
- [x] 1.2 Have `GameContentLoader` implement `IGameContentLoader`
- [x] 1.3 Update `GameEngine` constructor to depend on `IGameContentLoader` instead of `GameContentLoader`
- [x] 1.4 Update DI registration in `ServiceCollectionExtensions` to register `GameContentLoader` as `IGameContentLoader`

## 2. Test Infrastructure

- [x] 2.1 Add `BuildWorldContent(string startingRoomId)` method to `GameBuilder` that returns `WorldContent` from the built rooms with empty barriers
- [x] 2.2 Create `FakeGameContentLoader` in `tests/Questline.Tests/TestHelpers/` implementing `IGameContentLoader` — accepts `WorldContent` in constructor, returns it from `Load()`

## 3. Handle Null Input in Character Setup

- [x] 3.1 Update `CliApp.InitiateCharacterSetup` to return `Character?` and return `null` when `ReadLine()` returns null
- [x] 3.2 Update `CliApp.Run` to check for null character and exit gracefully (skip `LaunchGameSession` and `HandleGameLoop`)

## 4. Fix and Unskip CliAppTests

- [x] 4.1 Update `CliAppTests.CreateCliApp()` to use `GameBuilder.BuildWorldContent()`, `FakeGameContentLoader`, and construct `GameEngine` with the new signature
- [x] 4.2 Remove `Skip` attributes from all 5 tests blocked on `GameEngine.LaunchGame`
- [x] 4.3 Remove `Skip` attribute from `Null_input_exits_loop` test (blocked on null input handling)
- [x] 4.4 Remove `Skip` attribute from `Quit_command_exits_gracefully` test
- [x] 4.5 Run `dotnet build` and `dotnet test` to verify all 7 tests pass

## 5. Spec Sync

- [x] 5.1 Verify delta spec at `openspec/changes/unskip-cli-app-tests/specs/game-loop/spec.md` covers startup display changes (items, barriers) and EOF during character setup
