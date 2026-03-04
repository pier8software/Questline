---
name: "Testing"
description: "To design, implement, and maintain robust testing suites that ensure code reliability, performance, and regression safety."
---

# Writing a Test

## Principles

1. **Test First.** Write a failing test before writing any production code.
2. **Behaviour coverage, not code coverage.** Ask "what does this do?" not "what lines does this touch?". 100% code coverage is not a goal; 100% behaviour coverage is.
3. **Prefer integration tests.** Test the full handler or service stack. Only unit-test isolated logic (validators, value objects) when there is no higher-level entry point.
4. **No mocking frameworks.** Write Fakes and Stubs by hand. `Moq`, `NSubstitute`, and similar libraries are not used.

## Stack

- **xUnit** — test framework
- **Shouldly** — assertions

## TDD Workflow

1. Write a failing `[Fact]` or `[Theory]` that describes the behaviour you are about to add.
2. Run `dotnet test --no-build` and confirm it fails for the right reason.
3. Write the minimum production code to make it pass.
4. Refactor if needed, keeping tests green.

Never write production code without a failing test.

## What to Test

Test observable behaviour from the outside. Do not assert that internal implementation was called or validate a property has been set.

**internal implemetaion tests are brittle.** If a methods implementation changes, the test will fail.

```csharp
// BAD — brittle test
[Fact]
public void Repository_is_called_when_saving_playthrough()
{
    var playthrough = new Playthrough { ... };
    var repository = new FakePlaythroughRepository();
    var handler = new SavePlaythroughHandler(repository);}
    repository.SaveWasCalled.ShouldBeTrue();
}
```

**Property-bag tests have no value.** If a test creates an object and then asserts every field equals what was just assigned, it tests nothing. Flag it, explain why, and ask what behaviour the user actually wants to verify.

```csharp
// BAD — tests nothing
[Fact]
public void Barrier_stores_all_content_properties()
{
    var barrier = new Barrier { Id = "iron-door", Name = "iron door", ... };
    barrier.Id.ShouldBe("iron-door"); // just set this two lines up
}
```

A behaviour test asserts what the system *does* with the data:

```csharp
// GOOD — tests that a locked exit actually blocks movement
[Fact]
public async Task Locked_exit_blocks_player_and_returns_barrier_message()
{
    ...
    var error = result.ShouldBeOfType<ErrorResponse>();
    error.ErrorMessage.ShouldBe("The iron door is locked tight.");
    _fixture.Playthrough.Location.ShouldBe("start");
}
```

## Test Structure

Follow **Arrange–Act–Assert**. Use a class **constructor** to set up shared fixtures and the subject
under test once. Do not repeat construction inside every test method.

```csharp
// tests/Questline.Tests/Engine/Handlers/TakeItemCommandHandler/When_item_is_in_room.cs
namespace Questline.Tests.Engine.Handlers;

public class When_item_is_in_room
{
    private readonly TakeItemHandler _handler;
    private readonly GameFixture _fixture;
    private readonly Item _lamp;

    public When_item_is_in_room()
    {
        _lamp = Templates.Items.Lamp;
        _fixture = new ScenarioBuilder
            .WithRoom(Templates.Rooms.Cellar.WithItems([_lamp]))
            .Build();

        _handler = new TakeItemHandler(
            _fixture.Session, _fixture.PlaythroughRepository, _fixture.RoomRepository);
    }

    [Fact]
    public async Task Brass_lamp_is_taken_and_added_to_inventory()
    {
        var result = await _handler.Handle(new Requests.TakeItemCommand("brass lamp"));

        var response = result.ShouldBeOfType<Responses.ItemTakenResponse>();
        response.ItemName.ShouldBe("brass lamp");
        _fixture.Playthrough.Inventory.ShouldContain(_lamp);
    }
}
```

For **distinct scenarios** that need different setup, create a **separate file** in the subject
folder. Each file is its own class with its own constructor. For **small variations** of the same
scenario, inline them in the test method.

```
tests/Questline.Tests/Engine/Handlers/
    MovePlayerCommandHandler/
        When_exits_are_open.cs
        When_exit_has_a_locked_barrier.cs
    TakeItemCommandHandler/
        When_item_is_in_room.cs
```

```csharp
// tests/Questline.Tests/Engine/Handlers/MovePlayerCommandHandler/When_exit_has_a_locked_barrier.cs
namespace Questline.Tests.Engine.Handlers;

public class When_exit_has_a_locked_barrier
{
    private readonly MovePlayerCommandHandler _handler;
    private readonly GameFixture _fixture;

    public When_exit_has_a_locked_barrier()
    {
        _fixture = ScenarioBuilder
            .WithRoom(Templates.Rooms.StandardRoom.WithExit(Direction.North, Templates.Exits.EndWithLockedDoor))
            .WithRoom(Templates.Rooms.EndRoom.WithExit(Direction.South, Templates.Exits.Start))
            .StartingRoom(Templates.Rooms.StandardRoom)
            .Build();

        _handler = new MovePlayerCommandHandler(...);
    }

    [Fact]
    public async Task Locked_exit_blocks_player_and_returns_barrier_message() { ... }
}
```

When a handler is called only for its side effects, discard the return value explicitly:

```csharp
_ = await _handler.Handle(new Requests.MovePlayerCommand(Direction.North));
_fixture.Playthrough.Location.ShouldBe("end");
```

## Naming

**Folders:** `<Subject>/` — named after the class under test (e.g. `MovePlayerCommandHandler/`).

**Files & classes:** `When_<scenario>` — describes the setup/context being tested. Plain English,
underscores between words.

**Methods:** `<Behaviour_as_fact>` — states the expected outcome. Start with the outcome — never
with `Should`, `Test`, or `Verify`.

```
TakeItemCommandHandler/
    When_item_is_in_room.cs          → class When_item_is_in_room
    When_item_is_not_in_room.cs      → class When_item_is_not_in_room
```

```csharp
// When_item_is_in_room.cs
public class When_item_is_in_room
{
    [Fact] public async Task Brass_lamp_is_taken_and_added_to_inventory() { }

    [Fact] public async Task Item_name_matching_is_case_insensitive() { }
}

// When_item_is_not_in_room.cs
public class When_item_is_not_in_room
{
    [Fact] public async Task Returns_error_message() { }
}
```

## Assertions

One test covers one scenario. Assert everything that matters — response and state change — in the
same test. Use Shouldly's fluent assertions.

```csharp
result.ShouldBe(expected);
result.ShouldBeOfType<Responses.PlayerMovedResponse>();
collection.ShouldContain(item);
value.ShouldBeNull();
flag.ShouldBeTrue();
action.ShouldThrow<InvalidOperationException>();
```

## Development Environment

Integration tests that hit the database require the development environment to be running. Start
it with:

```
aspire run
```

See `docs/devenv.md` for details.

## Fakes and Test Infrastructure

Write Fakes and Stubs by hand. No mocking frameworks.

- `FakeConsole`, `FakeDice` — in `TestHelpers/`
- `FakePlaythroughRepository`, `FakeRoomRepository` — inside `GameBuilder.cs`
- `GameBuilder` / `RoomBuilder` — fluent builders for constructing game state

Keep fakes minimal — only implement what the tests actually need.

**Test data: use builders and templates.** Build test data through using fluent
builders (`GameBuilder`, `RoomBuilder`) and data templates.

### Example Builder

Uses the `TestStack.Dossier` library to build up test data.

```csharp
// tests/Questline.Tests/TestHelpers/Builders/RoomBuilder.cs
public class RoomBuilder : TestDatBuilder<Room, RoomBuilder>
{
    public RoomBuilder WithId(string id) =>
        Set(x => x.Id, id);

    public RoomBuilder WithName(string name) =>
        Set(x => x.Name, name);

    public RoomBuilder WithDescription(string description) =>
        Set(x => x.Description, description);

    public RoomBuilder WithItems(IEnumerable<Item> items) =>
        Set(x => x.Items, new List<Item>(items));

    ...
}
```

### Example Template

```csharp
// tests/Questline.Tests/TestHelpers/Builder/Templates/Rooms.cs
public class Rooms
{
    public static RoomBuilder StandardRoom =>
        new RoomBuilder()
            .WithId("14a678b5-26c3-40ac-b550-bce00c3cb88a")
            .WithName("Standard Room")
            .WithDescription("A standard room.")
            .WithItems([Templates.Items.Key]);
}
````

When the same test object is constructed in multiple places, use the builder and template classes rather than copy-pasting the initialiser:

```csharp
// tests/Questline.Tests/Engine/Handlers/DropItemCommandHandlerTests.cs
namespace Questline.Tests.Engine.Handlers;

public class DropItemCommandHandlerTests
{
    private readonly GameFixture _fixture;

    public DropItemCommandHandlerTests()
    {
        _fixture = ScenarioBuilder
        .WithRoom(Templates.Rooms.StandardRoom
            .WithItems([Templates.Items.Key]))
        .Build();
    }
}
```

## Input Variations

When the same behaviour applies to multiple inputs and the assertion is identical, use `[Theory]`
with `[InlineData]` instead of repeating `[Fact]` tests.

```csharp
[Theory]
[InlineData("brass lamp")]
[InlineData("BRASS LAMP")]
[InlineData("Brass Lamp")]
public async Task Item_name_matching_is_case_insensitive(string input)
{
    var result = await _handler.Handle(new Requests.TakeItemCommand(input));
    result.ShouldBeOfType<Responses.ItemTakenResponse>();
}
```

If the assertion or setup differs between inputs, write separate `[Fact]` tests.

## Before Committing

- [ ] All tests pass — `dotnet test`
- [ ] No `Console.WriteLine` debugging left behind
- [ ] No commented-out code
