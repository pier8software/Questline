## Context

The `game-loop` and `cli-run-modes` specs both require game mode to seed adventure content into the database before starting the game loop. However, the current implementation already does not seed in game mode — `GameApp` and `GameEngine` read from the database without seeding. The `deploy-content` mode handles all seeding via `ContentDeploymentApp`.

Despite this, `AddQuestlineEngine()` still registers `JsonFileLoader` and `IContentSeeder`/`ContentSeeder` — services that are never used during game mode. The specs also carry stale requirements that mandate game-mode seeding.

## Goals / Non-Goals

**Goals:**

- Align specs with the intended architecture: deploy-content mode owns content seeding, game mode consumes content from the database
- Remove unused `JsonFileLoader` and `IContentSeeder` registrations from the game engine service stack
- Establish a clean separation of concerns between deployment and gameplay

**Non-Goals:**

- Adding error handling when content is missing from the database
- Changing how `deploy-content` mode or `ContentDeploymentApp` works
- Modifying `GameEngine` or `GameApp` behaviour

## Decisions

### Remove content seeding services from `AddQuestlineEngine()`

Remove the `JsonFileLoader` and `IContentSeeder`/`ContentSeeder` registrations from `Engine/ServiceCollectionExtensions.AddQuestlineEngine()`. These services are only needed by `deploy-content` mode, which already registers them independently in `CliAppBuilder.ConfigureServices()`.

**Rationale**: Game mode never calls `IContentSeeder.SeedAdventure()`. Registering unused services adds confusion about responsibilities and increases the DI container surface unnecessarily.

**Alternative considered**: Keep registrations but document they're unused — rejected because dead registrations are misleading.

## Risks / Trade-offs

- **[Risk]** Developers running game mode without first deploying content will get repository errors instead of a clear message → Out of scope for this change; a future "missing content" error handling improvement can address this
- **[Risk]** Tests that depend on `IContentSeeder` being resolvable in the engine service stack will break → Mitigated by identifying and updating affected tests as part of implementation
