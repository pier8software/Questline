## 1. Setup

- [x] 1.1 Add `TestStack.Dossier` NuGet package to `tests/Questline.Tests/Questline.Tests.csproj`
- [x] 1.2 Create `TestHelpers/Builders/Templates/` directory

## 2. Entity Builders

- [x] 2.1 Refactor `RoomBuilder` to extend `TestDataBuilder<Room, RoomBuilder>` — replace primary-constructor pattern with Dossier `Set` API for Id, Name, Description; keep additive `WithExit`, `WithItem`, `WithFeature` methods with internal collections; override `BuildObject()` to merge
- [x] 2.2 Create `ItemBuilder` extending `TestDataBuilder<Item, ItemBuilder>` with `WithId`, `WithName`, `WithDescription`
- [x] 2.3 Create `BarrierBuilder` extending `TestDataBuilder<Barrier, BarrierBuilder>` with `WithId`, `WithName`, `WithDescription`, `WithBlockedMessage`, `WithUnlockItemId`, `WithUnlockMessage`
- [x] 2.4 Create `FeatureBuilder` extending `TestDataBuilder<Feature, FeatureBuilder>` with `WithId`, `WithName`, `WithKeywords`, `WithDescription`
- [x] 2.5 Create `ExitBuilder` extending `TestDataBuilder<Exit, ExitBuilder>` with `WithDestination`, `WithBarrier`
- [x] 2.6 Create `AdventureBuilder` extending `TestDataBuilder<Adventure, AdventureBuilder>` with `WithId`, `WithName`, `WithDescription`, `WithStartingRoomId`
- [x] 2.7 Create `PlaythroughBuilder` extending `TestDataBuilder<Playthrough, PlaythroughBuilder>` with fluent methods for all required properties, using sensible defaults (matching current `GameBuilder` defaults)

## 3. Template Classes

- [x] 3.1 Create `Templates/Items.cs` with static properties: `BrassLamp`, `RustyKey`, `Torch` — each returning a pre-configured `ItemBuilder`
- [x] 3.2 Create `Templates/Barriers.cs` with static property: `IronDoor` — returning a pre-configured `BarrierBuilder`
- [x] 3.3 Create `Templates/Features.cs` with static property: `StrangeSymbols` — returning a pre-configured `FeatureBuilder`
- [x] 3.4 Create `Templates/Rooms.cs` with static properties: `Cellar`, `Chamber`, `Hallway`, `Entrance`, `StartRoom`, `EndRoom`, `ThroneRoom`, `BeyondRoom` — each returning a pre-configured `RoomBuilder` with default exits where applicable

## 4. Refactor GameBuilder

- [x] 4.1 Add `WithRoom(RoomBuilder builder)` overload to `GameBuilder` that calls `builder.Build()` internally
- [x] 4.2 Update `GameBuilder.Build()` to accept a `RoomBuilder` as the starting room (or keep string-based start with builder-added rooms)

## 5. Update Handler Tests

- [x] 5.1 Update `DropItemCommandHandlerTests` — replace inline Item and room construction with Templates
- [x] 5.2 Update `ExamineCommandHandlerTests` — replace inline Item, Feature, and room construction with Templates
- [x] 5.3 Update `GetPlayerInventoryQueryHandlerTests` — replace inline Item and room construction with Templates
- [x] 5.4 Update `GetRoomDetailsHandlerTests` — replace inline Item, Barrier, and room construction with Templates
- [x] 5.5 Update `MovePlayerCommandHandlerTests` — replace inline Barrier and room construction with Templates
- [x] 5.6 Update `TakeItemHandlerTests` — replace inline Item and room construction with Templates
- [x] 5.7 Update `UseItemCommandHandlerTests` — replace `CreateBarrier()` helper and inline Items with Templates
- [x] 5.8 Update `GameEngineStartMenuTests` — replace inline Adventure, Room, and Playthrough construction with builders/Templates

## 6. Verification

- [x] 6.1 Run `dotnet build` — confirm no compilation errors
- [x] 6.2 Run `dotnet test` — confirm all existing tests pass with no assertion changes
- [x] 6.3 Remove any now-unused `CreateBarrier()` helper methods or dead code
