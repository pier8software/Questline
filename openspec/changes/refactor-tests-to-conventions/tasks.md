## 1. Remove Property-Bag Tests

- [x] 1.1 Delete `BarrierTests.cs` — pure property-bag test (`Barrier_stores_all_content_properties`)
- [x] 1.2 Delete `RoomTests.cs` — pure property-bag tests (`Room_stores_items_as_read_only`, `Exit_can_embed_barrier`)
- [x] 1.3 Audit `CharacterTests.cs` — remove property-bag assertions from `Create_sets_all_character_properties`, keep behavioural tests like `ToSummary_returns_character_summary`
- [x] 1.4 Audit `PlaythroughTests.cs` — remove property-bag test `Create_from_character_captures_all_data`, keep behavioural tests (`MoveTo_updates_location`, inventory operations, barrier unlocking)
- [x] 1.5 Run `dotnet test` to verify no regressions

## 2. Standardise Test Naming

- [x] 2.1 Rename `RequestsTests.cs` methods — replace `Can_create_*` prefix with behaviour-as-fact names (e.g., `DropItemCommand_is_created_from_args`)
- [x] 2.2 Rename `QuitGameHandlerTests` method — `Returns_quited_response` → `Returns_quit_response` (fix typo)
- [x] 2.3 Scan all test files for `Should_`/`Test_`/`Verify_`/`Can_` prefixes and rename to fact-based format
- [x] 2.4 Run `dotnet test` to verify no regressions

## 3. Hoist Shared Setup to Constructors

- [x] 3.1 Refactor `MovePlayerCommandHandlerTests` — extract shared setup into constructor; use nested classes for open-exit vs locked-barrier scenarios
- [x] 3.2 Refactor `DropItemCommandHandlerTests` — move shared `GameBuilder` setup to constructor
- [x] 3.3 Refactor `TakeItemHandlerTests` — move shared `GameBuilder` setup to constructor
- [x] 3.4 Refactor `ExamineCommandHandlerTests` — use nested classes per scenario (inventory item, room item, room feature)
- [x] 3.5 Refactor `UseItemCommandHandlerTests` — use nested classes per scenario (direct use, contextual use, wrong item)
- [x] 3.6 Refactor `GetRoomDetailsHandlerTests` — move shared setup to constructor
- [x] 3.7 Refactor `GetPlayerInventoryQueryHandlerTests` — move shared setup to constructor
- [x] 3.8 Run `dotnet test` to verify no regressions

## 4. Replace Inline Initialisers with Builders/Templates

- [x] 4.1 Refactor `CharacterTests.cs` — replace `new Character(...)` with builder/template if applicable
- [x] 4.2 Refactor `PlaythroughTests.cs` — replace inline `Playthrough` construction with `PlaythroughBuilder`
- [x] 4.3 Refactor `GameAppTests.cs` — replace inline room/adventure construction with builders and templates
- [x] 4.4 Refactor `GameEngineStartMenuTests.cs` — ensure all entity construction uses builders
- [x] 4.5 Run `dotnet test` to verify no regressions

## 5. Convert Duplicate Facts to Theories

- [x] 5.1 Audit `CharacterNameValidatorTests` — consolidate similar valid/invalid name tests into `[Theory]` where assertions are identical
- [x] 5.2 Audit all handler tests for repeated fact patterns that differ only in input
- [x] 5.3 Run `dotnet test` to verify no regressions

## 6. Final Verification

- [x] 6.1 Run full `dotnet test` suite and confirm all tests pass
- [x] 6.2 Verify no inline domain entity initialisers remain in test files (search for `new Room {`, `new Item {`, `new Barrier {`, `new Exit {`)
- [x] 6.3 Verify no test names start with `Should_`, `Test_`, `Verify_`, or `Can_`
