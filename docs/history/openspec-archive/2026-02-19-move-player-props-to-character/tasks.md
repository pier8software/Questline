## 1. Domain Model

- [x] 1.1 Write tests for `Character` class: has `Location` property, has `Inventory` property initialised to empty
- [x] 1.2 Create `Character` class in `Domain/Characters/Entity/Character.cs` with `required string Location` and `Inventory`
- [x] 1.3 Write tests for updated `Player` class: has `Character` property, no direct `Location` or `Inventory`
- [x] 1.4 Update `Player` class: remove `Location` and `Inventory`, add `required Character Character`

## 2. Test Infrastructure

- [x] 2.1 Update `GameBuilder.BuildState()` to create a `Character` with the `startLocation` and pass it to `Player`
- [x] 2.2 Verify all existing tests still compile after GameBuilder change

## 3. Handler Updates

- [x] 3.1 Update `MovePlayerCommandHandler`: change `state.Player.Location` to `state.Player.Character.Location`
- [x] 3.2 Update `GetRoomDetailsHandler`: change `state.Player.Location` to `state.Player.Character.Location`
- [x] 3.3 Update `DropItemCommandHandler`: change `state.Player.Inventory` and `state.Player.Location` to route through `state.Player.Character`
- [x] 3.4 Update `TakeItemHandler`: change `state.Player.Inventory` and `state.Player.Location` to route through `state.Player.Character`
- [x] 3.5 Update `ExamineCommandHandler`: change `state.Player.Inventory` and `state.Player.Location` to route through `state.Player.Character`
- [x] 3.6 Update `GetPlayerInventoryQueryHandler`: change `state.Player.Inventory` to `state.Player.Character.Inventory`
- [x] 3.7 Update `UseItemCommandHandler`: change `state.Player.Inventory` and `state.Player.Location` to route through `state.Player.Character`

## 4. Remaining References

- [x] 4.1 Search for any remaining references to `Player.Location` or `Player.Inventory` across the codebase and update them
- [x] 4.2 Check `GameState` constructor and any game loop / CLI code that creates a `Player` instance

## 5. Verification

- [x] 5.1 Run `dotnet build` — solution compiles with zero errors
- [x] 5.2 Run `dotnet test` — all existing tests pass
