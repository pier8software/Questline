## Why

The Questline application currently has a single execution path: start the game loop. As the project evolves toward a platform, operational tasks like deploying game content to the database need to run from the same binary without launching the interactive game. A CLI run-mode system lets the application serve multiple purposes — game play by default, and administrative operations via flags — keeping deployment simple (one artifact, many capabilities).

## What Changes

- Introduce a `--mode` command-line argument that selects the application's execution mode
- **Default (no args)**: run the game as today — seed content, start the game loop
- **`--mode=deploy-content`**: load adventure JSON files and upsert them into MongoDB, then exit — no game loop, no interactive I/O
- **BREAKING**: `CliAppBuilder.Build()` currently seeds content unconditionally during startup; this will move into the game mode path so that content deployment is explicit and controlled
- Extract content seeding from the build pipeline into a standalone operation that can be invoked independently

## Capabilities

### New Capabilities

- `cli-run-modes`: Defines how the application parses `--mode` arguments, routes to the correct execution path, and what modes are supported. Covers argument parsing, mode dispatch, and the deploy-content mode behaviour.

### Modified Capabilities

- `game-loop`: The game loop startup changes — content seeding is no longer implicit during build; the game mode must ensure content is available before starting play.
- `content-loading`: Content deployment becomes an explicit operation (deploy-content mode) rather than a side-effect of application startup.

## Non-goals

- Generic CLI framework or argument parsing library (e.g. System.CommandLine) — a simple switch on `--mode` is sufficient for now
- Additional modes beyond `deploy-content` in this change — future modes (e.g. `--mode=migrate`) can follow the same pattern later
- Changing how content JSON files are structured or validated
- Making the deploy-content mode idempotent or transactional beyond what MongoDB upserts already provide

## Impact

- **`Cli/`**: `Program.cs` gains argument parsing; `CliAppBuilder` splits into mode-aware setup
- **`Engine/Content/`**: `ContentSeeder` usage moves from implicit build step to explicit invocation
- **`Framework/`**: No changes expected — MongoDB persistence layer stays the same
- **Tests**: New tests for argument parsing and mode dispatch; existing game loop tests may need minor updates to account for separated seeding
- **DevEnv**: No changes — Aspire orchestration is unaffected
