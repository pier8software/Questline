## 1. Tests

- [x] 1.1 Write test: game mode service container does not resolve `IContentSeeder` — build `CliAppBuilder` in game mode, assert `GetService<IContentSeeder>()` returns null
- [x] 1.2 Write test: game mode service container does not resolve `JsonFileLoader` — build `CliAppBuilder` in game mode, assert `GetService<JsonFileLoader>()` returns null

## 2. Remove content seeding registrations from game engine

- [x] 2.1 Remove `JsonFileLoader` registration from `Engine/ServiceCollectionExtensions.AddQuestlineEngine()`
- [x] 2.2 Remove `IContentSeeder`/`ContentSeeder` registration from `Engine/ServiceCollectionExtensions.AddQuestlineEngine()`
- [x] 2.3 Remove unused `using` directives from `ServiceCollectionExtensions.cs` if any remain after the removals

## 3. Verify

- [x] 3.1 Run `dotnet build` — confirm solution compiles without errors
- [x] 3.2 Run `dotnet test --no-build` — confirm all tests pass
