## 1. Domain: Exit value object

- [x] 1.1 Create `Exit` record in `Domain/Exit.cs` — `public record Exit(string Destination, string? BarrierId = null);` Write a test that verifies Exit equality by value and that BarrierId defaults to null.
- [x] 1.2 Change `Room.Exits` from `Dictionary<Direction, string>` to `Dictionary<Direction, Exit>`. Update the Room class and verify existing Room tests still compile.

## 2. WorldBuilder update

- [x] 2.1 Update `RoomBuilder` to store `Dictionary<Direction, Exit>` internally. Add `WithExit(Direction, Exit)` overload and make the existing `WithExit(Direction, string)` delegate to it. Write a test that builds a room with a barrier exit and asserts BarrierId is set.

## 3. Handler updates

- [x] 3.1 Update `GoCommandHandler` to read `exit.Destination` instead of the raw string. Update existing Go handler tests to work with the new Exit type — no behavioural change expected.
- [x] 3.2 Update `LookCommandHandler` — it reads `room.Exits.Keys` which is unchanged, but verify compilation and tests pass.

## 4. Content loader update

- [x] 4.1 Update `FileSystemAdventureLoader.BuildRooms` to produce `Dictionary<Direction, Exit>` from `ExitDto`, mapping `ExitDto.Barrier` → `Exit.BarrierId`. Update content loading tests to assert that barrier references from JSON flow through to `Exit.BarrierId`.

## 5. Verification

- [x] 5.1 Run full test suite (`dotnet test`) — all existing tests pass with no behavioural change.
- [x] 5.2 Run build (`dotnet build`) — no warnings.
