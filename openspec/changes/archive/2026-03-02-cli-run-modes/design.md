## Context

The application has a single execution path: `Program.cs` → `CliAppBuilder` → `CliApp.Run()`. Content seeding is baked into `CliAppBuilder.Build()`, meaning every startup seeds the database before launching the game loop. There is no way to run operational tasks (like deploying content) without also starting the interactive game.

`Program.cs` currently ignores `args` entirely. The composition root (`CliAppBuilder`) couples content seeding with game startup, and the `CliApp` class only knows how to run the game loop.

## Goals / Non-Goals

**Goals:**

- Parse a `--mode` argument from the command line to select an execution path
- Support two modes: `game` (default) and `deploy-content`
- Separate content seeding from game loop startup so each can run independently
- Keep the design simple and extensible for future modes without over-engineering

**Non-Goals:**

- Adopting `System.CommandLine` or any CLI parsing library
- Supporting multiple simultaneous modes or mode composition
- Changing `ContentSeeder` internals or MongoDB persistence behaviour
- Configuration files or environment variables for mode selection

## Decisions

### 1. Introduce a `RunMode` enum and parse `--mode` in `Program.cs`

**Decision**: Define a `RunMode` enum (`Game`, `DeployContent`) in `Cli/`. Parse `args` in `Program.cs` with a simple helper method that scans for `--mode=<value>`. Default to `Game` when no `--mode` argument is present.

**Rationale**: Keeps argument parsing in the entry point where it belongs. A dedicated enum makes mode values type-safe and discoverable. A simple args scanner avoids pulling in a library dependency for one flag.

**Alternatives considered**:
- `System.CommandLine`: Too heavy for a single flag; adds dependency and ceremony
- String matching in `Program.cs` without an enum: Loses type safety, harder to extend

### 2. Replace `CliAppBuilder` with mode-aware builder methods

**Decision**: Refactor `CliAppBuilder` so that `Build()` no longer seeds content. Instead, expose separate build methods or a mode parameter that configures only the services needed for the selected mode. For `Game` mode, register the full game stack (console, engine, game loop) and seed content. For `DeployContent` mode, register only persistence and content seeding services — no console, no game engine.

**Rationale**: Each mode has different dependencies. The game needs `IConsole`, `GameEngine`, `CliApp`; content deployment only needs `ContentSeeder` and persistence. Building only what's needed keeps startup fast and avoids resolving unnecessary services.

**Approach**: Add a `Build(RunMode mode)` method. For `Game`, it configures all services, seeds content, and returns a runnable app. For `DeployContent`, it configures persistence + seeder, runs the seed, and returns a no-op or signals completion directly.

### 3. Use a mode runner abstraction for dispatch

**Decision**: Introduce an `IRunMode` interface with a single `Task RunAsync()` method. Each mode gets its own implementation: `GameMode` (wraps the existing `CliApp.Run()` flow including seeding) and `DeployContentMode` (seeds content, writes confirmation to console, exits). `Program.cs` resolves the appropriate `IRunMode` and calls `RunAsync()`.

**Rationale**: Cleanly separates dispatch from execution. Adding a future mode means adding one class — no switch statement in `Program.cs` grows unboundedly. Each mode owns its full lifecycle.

**Alternatives considered**:
- Switch statement in `Program.cs` calling different methods: Works for two modes but grows messy; mixes orchestration with entry point logic
- Separate executables per mode: Defeats the "one binary, many capabilities" goal

### 4. Content seeding moves into mode implementations

**Decision**: Remove the `seeder.SeedAdventure()` call from `CliAppBuilder.Build()`. `GameMode.RunAsync()` seeds content before starting the game loop. `DeployContentMode.RunAsync()` seeds content and then exits.

**Rationale**: Seeding is now an explicit action owned by the mode that needs it, not a side-effect of building the service container. This makes the application's behaviour predictable based on the mode argument alone.

## Risks / Trade-offs

- **Game mode still seeds on every startup** → This maintains current behaviour. Future work could check if content is already seeded, but that's out of scope per proposal non-goals.
- **No validation of `--mode` values at parse time** → Mitigated by the enum parser; invalid values produce a clear error message and exit with non-zero code.
- **Breaking change for anyone scripting `dotnet run`** → Low risk since the default (no args) preserves existing behaviour. Only callers that relied on seeding as a build side-effect would notice, and there are no known external consumers.
