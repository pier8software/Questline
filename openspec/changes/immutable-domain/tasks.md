## 1. Immutable Inventory

- [x] 1.1 Write tests for immutable `Inventory`: `Add` returns new instance with item, `Remove` returns new instance without item, originals unchanged, `FindByName` and `Contains` queries work on new instances
- [x] 1.2 Convert `Inventory` to a record backed by `ImmutableList<Item>`, `Add`/`Remove` return new `Inventory` instances

## 2. Immutable Room

- [x] 2.1 Write tests for immutable `Room`: `AddItem` returns new `Room` with item in `Items`, `RemoveItem` returns new `Room` without item, originals unchanged
- [x] 2.2 Convert `Room` to a record with `ImmutableDictionary<Direction, Exit>` for `Exits`, `Inventory` for `Items`, and `ImmutableList<Feature>` for `Features`; add `AddItem`/`RemoveItem` methods returning new instances

## 3. Immutable Barrier

- [x] 3.1 Write tests for immutable `Barrier`: `Unlock()` returns new `Barrier` with `IsUnlocked` true, original retains `IsUnlocked` false
- [x] 3.2 Convert `Barrier` to a record with `init`-only properties; add `Unlock()` method returning a new instance

## 4. Immutable Character

- [x] 4.1 Write tests for immutable `Character`: `MoveTo` returns new instance with updated `Location`, `AddInventoryItem` returns new instance with item in inventory, `RemoveInventoryItem` returns new instance without item; originals unchanged
- [x] 4.2 Update `Character` record: replace `SetLocation` with `MoveTo` returning new instance, add `AddInventoryItem`/`RemoveInventoryItem` returning new instances, make `Location` and `Inventory` init-only

## 5. GameState Update Methods

- [x] 5.1 Write tests for `GameState.UpdatePlayer`, `UpdateRoom`, and `UpdateBarrier`: each replaces the entity and subsequent lookups return the new instance
- [x] 5.2 Add `UpdatePlayer`, `UpdateRoom`, `UpdateBarrier` methods to `GameState`

## 6. Refactor Handlers

- [x] 6.1 Update `MovePlayerCommandHandler` to use `Character.MoveTo` and `state.UpdatePlayer`; update handler tests
- [x] 6.2 Update `TakeItemHandler` to use `Room.RemoveItem`, `Character.AddInventoryItem`, `state.UpdateRoom`, and `state.UpdatePlayer`; update handler tests
- [x] 6.3 Update `DropItemCommandHandler` to use `Character.RemoveInventoryItem`, `Room.AddItem`, `state.UpdateRoom`, and `state.UpdatePlayer`; update handler tests
- [x] 6.4 Update `UseItemCommandHandler` to use `Barrier.Unlock()` and `state.UpdateBarrier`; update handler tests

## 7. Update Test Helpers and Remaining Tests

- [x] 7.1 Update `RoomBuilder` to construct immutable `Room` records with immutable collections
- [x] 7.2 Update `GameBuilder` to work with immutable entity construction patterns
- [x] 7.3 Update any remaining domain entity tests (`CharacterTests`, `InventoryTests`, `BarrierTests`) to assert immutability
- [x] 7.4 Run full test suite (`dotnet test`) and fix any compilation or assertion failures
