## 1. Foundation: Barrier, Feature, GameState, data layer

- [x] 1.1 Create `Barrier` entity in `Domain/Rooms/Entity/Barrier.cs` with `Id`, `Name`, `Description`, `BlockedMessage`, `UnlockItemId`, `UnlockMessage` (all required init), `IsUnlocked` (mutable). Write tests: starts locked, can be unlocked.
- [x] 1.2 Create `Feature` entity in `Domain/Rooms/Entity/Feature.cs` with `Id`, `Name`, `Keywords` (string[]), `Description`. Write test: has properties and keywords.
- [x] 1.3 Create `BarrierData` and `FeatureData` DTOs in `Domain/Rooms/Data/`.
- [x] 1.4 Extend `GameState` with optional barriers dictionary parameter, `Barriers` property, and `GetBarrier(string? id)` method. Write tests: found, not found, null id.
- [x] 1.5 Add `BarrierData[] Barriers` to `AdventureData`, `FeatureData[] Features` to `RoomData`, `List<Feature> Features` to `Room`.
- [x] 1.6 Update `GameBuilder` with `WithBarrier(Barrier)` and `BuildState(playerId, startLocation)`. Update `RoomBuilder` with `WithFeature(Feature)`.

## 2. Barriers block movement

- [x] 2.1 Write tests in `MovePlayerCommandHandlerTests`: locked barrier blocks, unlocked allows, no barrier allows.
- [x] 2.2 Implement barrier check in `MovePlayerCommandHandler` — after exit lookup, before moving, check `exit.BarrierId` and `barrier.IsUnlocked`.

## 3. Look shows locked barriers

- [x] 3.1 Write tests in `GetRoomDetailsHandlerTests`: locked barrier description included, unlocked barrier omitted.
- [x] 3.2 Add optional `lockedBarriers` parameter to `RoomDetailsResponse.Success` and `PlayerMovedResponse.Success` formatting.
- [x] 3.3 Update `GetRoomDetailsHandler` and `MovePlayerCommandHandler` to gather and pass locked barrier descriptions.

## 4. Use command

- [x] 4.1 Create `UseItemCommand` with `[Verbs("use")]` and `CreateRequest` splitting on `" on "`.
- [x] 4.2 Create `UseItemResponse` with Success/Error factories.
- [x] 4.3 Write handler tests: correct item unlocks, wrong item errors, not in inventory, contextual use, target not found, already unlocked.
- [x] 4.4 Implement `UseItemCommandHandler`: find item in inventory, find target barrier (targeted or contextual), validate, unlock.
- [x] 4.5 Register handler in `ServiceCollectionExtensions`. Write parser tests for `use` command.

## 5. Examine command

- [x] 5.1 Create `ExamineCommand` with `[Verbs("examine", "inspect")]` and `ExamineResponse`.
- [x] 5.2 Write handler tests: examine inventory item, room item, room feature by keyword, room feature by name, unknown target.
- [x] 5.3 Implement `ExamineCommandHandler`: search inventory items → room items → room features by name/keywords.
- [x] 5.4 Register handler in `ServiceCollectionExtensions`. Write parser tests for `examine` command.

## 6. Content loading and adventure JSON

- [x] 6.1 Add `barriers` array to `the-goblins-lair.json` with iron-door definition. Add `features` to puzzle-room with strange symbols.
- [x] 6.2 Update `GameContentLoader` to build `Dictionary<string, Barrier>` from `adventureData.Barriers`, build `List<Feature>` per room, pass barriers to `GameState` constructor.

## 7. Verification

- [x] 7.1 `dotnet build` — compiles with no warnings.
- [x] 7.2 `dotnet test` — all tests pass.
