## 1. Add Username to Playthrough Domain and Persistence

- [x] 1.1 Add `Username` property to `Playthrough` entity and update `Playthrough.Create` factory to accept a username parameter
- [x] 1.2 Add `Username` field to `PlaythroughDocument` and update `PlaythroughMapper` to map it both directions
- [x] 1.3 Write tests for `Playthrough.Create` verifying username is stored correctly

## 2. Extend GameSession with Username

- [x] 2.1 Add `Username` property and `SetUsername` method to `IGameSession` and `GameSession`
- [x] 2.2 Update `GameEngine.HandleLogin` to store the username on `GameSession` after successful login
- [x] 2.3 Update `GameEngine.HandleCharacterCreation` to pass `gameSession.Username` when creating the `Playthrough`

## 3. Add FindByUsername to PlaythroughRepository

- [x] 3.1 Write tests for `PlaythroughRepository.FindByUsername` returning matching playthrough summaries and an empty list when none exist
- [x] 3.2 Add a `PlaythroughSummary` record (Id, CharacterName, AdventureId, Location) to the domain or engine layer
- [x] 3.3 Add `FindByUsername(string username)` to `IPlaythroughRepository` returning `Task<IReadOnlyList<PlaythroughSummary>>`
- [x] 3.4 Implement `FindByUsername` in `PlaythroughRepository` using a MongoDB filter on the `Username` field

## 4. Add StartMenu Phase

- [x] 4.1 Add `StartMenu` and `LoadGame` values to the `GamePhase` enum
- [x] 4.2 Add `StartMenuResponse` response record with the two menu options
- [x] 4.3 Write tests for `GameEngine` verifying: login transitions to `StartMenu`, selecting "1" transitions to `NewAdventure`, selecting "2" transitions to `LoadGame`, invalid input returns error and stays on `StartMenu`
- [x] 4.4 Implement `HandleStartMenu` in `GameEngine` that parses "1"/"2" input and transitions to the correct phase
- [x] 4.5 Update `HandleLogin` to transition to `StartMenu` instead of `NewAdventure`
- [x] 4.6 Add `StartMenu` case to `GameEngine.ProcessInput` switch

## 5. Add LoadGame Phase

- [x] 5.1 Add `SavedPlaythroughsResponse` response record containing the list of playthrough summaries
- [x] 5.2 Add `NoSavedGamesResponse` response record for the empty case
- [x] 5.3 Write tests for `GameEngine` verifying: entering `LoadGame` phase queries playthroughs by username, displays list, selecting a valid playthrough loads state and transitions to `Playing`, no saved games returns to `StartMenu`, invalid selection returns error
- [x] 5.4 Implement `HandleLoadGame` in `GameEngine` — on first entry query playthroughs and display list, on subsequent input parse numeric selection and load the playthrough
- [x] 5.5 Add `LoadGame` case to `GameEngine.ProcessInput` switch

## 6. Resume Adventure from Loaded Playthrough

- [x] 6.1 Write tests verifying a loaded playthrough sets `PlaythroughId` on `GameSession`, transitions to `Playing`, and returns the current room details via `StartAdventure`
- [x] 6.2 Implement the resume logic in `HandleLoadGame` — call `playthroughRepository.GetById`, set session playthrough ID, call `StartAdventure`, transition to `Playing`

## 7. Update Response Formatting

- [x] 7.1 Update `ResponseFormatter` to handle `StartMenuResponse`, `SavedPlaythroughsResponse`, and `NoSavedGamesResponse`
- [x] 7.2 Manually test the full flow: login → start menu → new game, and login → start menu → load game → resume

## 8. Finalize

- [x] 8.1 Run full test suite and fix any regressions
- [x] 8.2 Update `CHANGELOG.md` and `<Version>` in `Directory.Build.props`
