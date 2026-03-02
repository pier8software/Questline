## 1. RunMode Enum and Argument Parsing

- [ ] 1.1 Write tests for `--mode` argument parsing: no args defaults to `Game`, `--mode=game` returns `Game`, `--mode=deploy-content` returns `DeployContent`, `--mode=invalid` throws or returns error
- [ ] 1.2 Create `RunMode` enum (`Game`, `DeployContent`) in `Cli/`
- [ ] 1.3 Create argument parser helper that scans `args` for `--mode=<value>` and returns a `RunMode`

## 2. IRunMode Abstraction

- [ ] 2.1 Create `IRunMode` interface in `Cli/` with a single `Task RunAsync()` method

## 3. GameMode Implementation

- [ ] 3.1 Write tests for `GameMode`: verifies content is seeded and game loop is started
- [ ] 3.2 Create `GameMode` class implementing `IRunMode` — seeds content via `ContentSeeder`, then delegates to `CliApp.Run()`

## 4. DeployContentMode Implementation

- [ ] 4.1 Write tests for `DeployContentMode`: verifies content is seeded, confirmation message is written, and no game loop is started
- [ ] 4.2 Create `DeployContentMode` class implementing `IRunMode` — seeds content via `ContentSeeder`, writes confirmation to console, exits

## 5. Refactor CliAppBuilder

- [ ] 5.1 Remove `ContentSeeder.SeedAdventure()` call from `CliAppBuilder.Build()`
- [ ] 5.2 Add `RunMode` parameter to `Build()` (or split into mode-aware configuration) so only required services are registered per mode
- [ ] 5.3 For `Game` mode: register full game stack (`IConsole`, `GameEngine`, `CliApp`, `GameMode`)
- [ ] 5.4 For `DeployContent` mode: register only persistence, `ContentSeeder`, and `DeployContentMode`

## 6. Update Program.cs Entry Point

- [ ] 6.1 Parse `args` to determine `RunMode` in `Program.cs`
- [ ] 6.2 Pass `RunMode` to `CliAppBuilder`, resolve `IRunMode`, and call `RunAsync()`
- [ ] 6.3 Handle invalid mode: display error message listing valid modes and exit with non-zero code

## 7. Verify and Clean Up

- [ ] 7.1 Run full test suite and verify existing game loop tests still pass
- [ ] 7.2 Manually verify `dotnet run --project src/Questline` starts the game (default mode)
- [ ] 7.3 Manually verify `dotnet run --project src/Questline -- --mode=deploy-content` seeds content and exits
