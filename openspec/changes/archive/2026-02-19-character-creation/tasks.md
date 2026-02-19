## 1. Domain Models

- [x] 1.1 Add `Race` enum (`Human`, `Elf`, `Dwarf`, `Halfling`) to `Domain/Characters/Entity/`
- [x] 1.2 Add `CharacterClass` enum (`Fighter`, `Wizard`, `Rogue`, `Cleric`) to `Domain/Characters/Entity/`
- [x] 1.3 Add `CharacterStats` record (`MaxHealth`, `CurrentHealth`, `STR`, `INT`, `WIS`, `DEX`, `CON`, `CHA`) to `Domain/Characters/Entity/`
- [x] 1.4 Add `Character` record (`Name`, `Race`, `Class`, `Level`, `Experience`, `Stats`) to `Domain/Characters/Entity/`
- [x] 1.5 Add `Character` property to `Player` entity

## 2. Dice Rolling

- [x] 2.1 Add `IDice` interface with `int Roll(int sides)` method in `Domain/Characters/Entity/`
- [x] 2.2 Add `Dice` production implementation using `Random.Shared`
- [x] 2.3 Add `FakeDice` test double returning deterministic values

## 3. Name Validation

- [x] 3.1 Write tests for character name validation (valid name, empty, too short, too long, special chars, whitespace)
- [x] 3.2 Implement `CharacterNameValidator` in `Domain/Characters/` — static validation method returning success/error

## 4. Character Factory

- [x] 4.1 Write tests for `CharacterFactory` — validates name, rolls 3d6 for each ability score, sets MaxHealth to 8, rolls 1d8 for CurrentHealth, defaults to Human Fighter
- [x] 4.2 Implement `CharacterFactory` in `Domain/Characters/` — accepts `IDice` and name, returns `Character`

## 5. Stats Command

- [x] 5.1 Add `StatsQuery` request record with `[Verbs("stats")]` to `Engine/Messages/Requests.cs`
- [x] 5.2 Add `StatsResponse` response record with `Message` containing name, race, class, level, and stats to `Engine/Messages/Responses.cs`
- [x] 5.3 Write tests for `StatsQueryHandler` — returns formatted character info
- [x] 5.4 Implement `StatsQueryHandler` in `Engine/Handlers/`
- [x] 5.5 Register `StatsQueryHandler` in `Engine/ServiceCollectionExtensions.RegisterCommandHandlers()`

## 6. Game Start Flow

- [x] 6.1 Write tests for game start flow — prompts for name, displays welcome with character name, then shows starting room
- [x] 6.2 Modify `CliApp.Run()` to prompt for character name, create character via `CharacterFactory`, set on Player, and display welcome message before entering game loop
- [x] 6.3 Update `GameState`/`CliAppBuilder` to support deferred Player construction (Player created after name entry)
- [x] 6.4 Register `IDice` and `CharacterFactory` in DI container

## 7. Test Helpers

- [x] 7.1 Update `GameBuilder` to support setting a `Character` on the test `Player`
