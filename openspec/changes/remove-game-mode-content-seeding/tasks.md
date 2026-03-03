## 1. Tests

- [ ] 1.1 Write test: game mode service container does not resolve `IContentSeeder` — build `CliAppBuilder` in game mode, assert `GetService<IContentSeeder>()` returns null
- [ ] 1.2 Write test: game mode service container does not resolve `JsonFileLoader` — build `CliAppBuilder` in game mode, assert `GetService<JsonFileLoader>()` returns null

## 2. Remove content seeding registrations from game engine

- [ ] 2.1 Remove `JsonFileLoader` registration from `Engine/ServiceCollectionExtensions.AddQuestlineEngine()`
- [ ] 2.2 Remove `IContentSeeder`/`ContentSeeder` registration from `Engine/ServiceCollectionExtensions.AddQuestlineEngine()`
- [ ] 2.3 Remove unused `using` directives from `ServiceCollectionExtensions.cs` if any remain after the removals

## 3. Verify

- [ ] 3.1 Run `dotnet build` — confirm solution compiles without errors
- [ ] 3.2 Run `dotnet test --no-build` — confirm all tests pass
