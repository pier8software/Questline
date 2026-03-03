## Why

The `game-loop` and `cli-run-modes` specs both require game mode to seed adventure content into the database before starting the game loop. In practice, `deploy-content` mode is always executed before `game` mode, so the content is already available in the database. Seeding in game mode is redundant and couples the game startup to content loading concerns that belong exclusively to the deployment pipeline.

## What Changes

- Remove the requirement for game mode to seed adventure content — game mode will assume content is already present in the database
- Remove `IContentSeeder` and `JsonFileLoader` registrations from the game mode service stack (`AddQuestlineEngine`)
- Update specs to reflect that content seeding is the sole responsibility of `deploy-content` mode

## Capabilities

### New Capabilities

_None._

### Modified Capabilities

- `game-loop`: Remove the requirement that adventure content is seeded as part of game mode startup
- `cli-run-modes`: Remove the requirement that game mode seeds adventure content; clarify that deploy-content mode is the sole mechanism for seeding

## Non-goals

- Changing how `deploy-content` mode works — it continues to seed content as-is
- Adding runtime checks or error messages when content is missing from the database — this is a separate concern
- Modifying the `GameEngine`, `GameApp`, or game loop flow — only DI registrations and specs change

## Impact

- **Specs**: `game-loop/spec.md` and `cli-run-modes/spec.md` updated to remove game-mode seeding requirements
- **Code**: `Engine/ServiceCollectionExtensions.cs` — remove `JsonFileLoader` and `IContentSeeder` registrations from `AddQuestlineEngine()`
- **Tests**: Any tests asserting game-mode content seeding will need updating or removal
