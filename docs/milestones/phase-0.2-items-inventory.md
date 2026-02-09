# Phase 0.2: Items and Inventory

## Status

Not Started

## Objective

> Add items to the world that players can pick up, carry, and drop. This lays the groundwork for puzzle mechanics and gear while proving the command pipeline supports multi-entity interaction.

## Acceptance Criteria

### Item Entity
- [ ] Items exist as entities with required `Id`, `Name`, and `Description` properties
- [ ] Items use `required init` properties following the Room entity pattern

### Inventory Container
- [ ] Inventory can add an item
- [ ] Inventory can remove an item (returns `bool`)
- [ ] Inventory can find an item by name (case-insensitive exact match)
- [ ] Inventory can check whether it contains a specific item
- [ ] Inventory exposes items as a readonly collection
- [ ] Inventory reports whether it is empty

### Room Items
- [ ] Rooms have an `Items` property backed by `Inventory`
- [ ] `WorldBuilder` supports adding items to rooms via `RoomBuilder.WithItem`

### Player Inventory
- [ ] Player has an `Inventory` property
- [ ] Player inventory starts empty

### Get Command
- [ ] `get <item>` picks up item from current room and adds to player inventory
- [ ] `get <item>` removes the item from the room
- [ ] `get <item>` returns a success result with descriptive message
- [ ] `get <item>` returns an error if item is not present in room
- [ ] `get` with no argument returns an error asking what to pick up
- [ ] `take` works as an alias for `get`
- [ ] Item name matching is case-insensitive

### Drop Command
- [ ] `drop <item>` removes item from player inventory and places in current room
- [ ] `drop <item>` returns a success result with descriptive message
- [ ] `drop <item>` returns an error if item is not in inventory
- [ ] `drop` with no argument returns an error asking what to drop

### Inventory Command
- [ ] `inventory` lists all items currently carried
- [ ] `inv` and `i` work as aliases for `inventory`
- [ ] Inventory command shows a message when carrying nothing

### Look Enhancement
- [ ] `look` result includes items visible in the current room
- [ ] `go` result includes items visible in the destination room
- [ ] Items are hidden from room output when none are present

## Implementation Notes

### Suggested Order

1. Define `Item` entity in Domain
2. Implement `Inventory` container in Domain with tests
3. Add `Items` property to `Room` (backed by `Inventory`)
4. Add `Inventory` property to `Player`
5. Extend `RoomBuilder.WithItem` and `WorldBuilder` integration
6. Define `GetCommand`, `DropCommand`, `InventoryCommand` records
7. Define result types: `ItemPickedUpResult`, `ItemDroppedResult`, `InventoryResult`
8. Update `LookResult` and `MovedResult` to include items (breaking change)
9. Implement `GetCommandHandler` with tests
10. Implement `DropCommandHandler` with tests
11. Implement `InventoryCommandHandler` with tests
12. Update `LookCommandHandler` and `GoCommandHandler` to include room items
13. Register new commands in `Program.cs`

### Item Entity

```csharp
namespace Questline.Domain;

public class Item
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}
```

### Inventory Container

Model inventory as a reusable container. Both `Room.Items` and `Player.Inventory` use this same abstraction. Party inventory (Phase 2+) will reuse it.

```csharp
namespace Questline.Domain;

public class Inventory
{
    private readonly List<Item> _items = new();

    public IReadOnlyList<Item> Items => _items;
    public bool IsEmpty => _items.Count == 0;

    public void Add(Item item) => _items.Add(item);

    public bool Remove(Item item) => _items.Remove(item);

    public Item? FindByName(string name) =>
        _items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public bool Contains(Item item) => _items.Contains(item);
}
```

### Room and Player Changes

Add `Items` to Room:

```csharp
public class Room
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public Dictionary<Direction, string> Exits { get; init; } = new();
    public Inventory Items { get; init; } = new();
}
```

Add `Inventory` to Player:

```csharp
public class Player
{
    public required string Id { get; init; }
    public required string Location { get; set; }
    public Inventory Inventory { get; init; } = new();
}
```

### WorldBuilder Extension

Add `WithItem` to `RoomBuilder`:

```csharp
public class RoomBuilder(string id, string name, string description)
{
    private readonly Dictionary<Direction, string> _exits = new();
    private readonly List<Item> _items = new();

    public RoomBuilder WithExit(Direction direction, string destinationId)
    {
        _exits[direction] = destinationId;
        return this;
    }

    public RoomBuilder WithItem(Item item)
    {
        _items.Add(item);
        return this;
    }

    public Room Build()
    {
        var room = new Room
        {
            Id = id,
            Name = name,
            Description = description,
            Exits = new Dictionary<Direction, string>(_exits)
        };

        foreach (var item in _items)
        {
            room.Items.Add(item);
        }

        return room;
    }
}
```

### Command Records

```csharp
namespace Questline.Engine.Commands;

public record GetCommand(string ItemName) : ICommand;
public record DropCommand(string ItemName) : ICommand;
public record InventoryCommand : ICommand;
```

### Result Types

```csharp
public record ItemPickedUpResult(string ItemName)
    : CommandResult($"You pick up the {ItemName}.");

public record ItemDroppedResult(string ItemName)
    : CommandResult($"You drop the {ItemName}.");

public record InventoryResult(IReadOnlyList<string> Items)
    : CommandResult(Items.Count == 0
        ? "You are not carrying anything."
        : $"You are carrying: {string.Join(", ", Items)}");
```

### Updated LookResult and MovedResult

This is a **breaking change** — existing tests for `look` and `go` will need updating to pass the new `Items` parameter.

```csharp
public record LookResult(string RoomName, string Description, IReadOnlyList<string> Exits, IReadOnlyList<string> Items)
    : CommandResult(FormatRoomDescription(RoomName, Description, Exits, Items))
{
    private static string FormatRoomDescription(
        string roomName, string description, IReadOnlyList<string> exits, IReadOnlyList<string> items)
    {
        var parts = new List<string> { roomName, description };

        if (items.Count > 0)
            parts.Add($"You can see: {string.Join(", ", items)}");

        parts.Add($"Exits: {string.Join(", ", exits)}");

        return string.Join("\n", parts);
    }
}

public record MovedResult(string RoomName, string Description, IReadOnlyList<string> Exits, IReadOnlyList<string> Items)
    : CommandResult(FormatRoomDescription(RoomName, Description, Exits, Items))
{
    private static string FormatRoomDescription(
        string roomName, string description, IReadOnlyList<string> exits, IReadOnlyList<string> items)
    {
        var parts = new List<string> { roomName, description };

        if (items.Count > 0)
            parts.Add($"You can see: {string.Join(", ", items)}");

        parts.Add($"Exits: {string.Join(", ", exits)}");

        return string.Join("\n", parts);
    }
}
```

### Command Handlers

**GetCommandHandler:**

```csharp
using Questline.Domain;
using Questline.Engine.Commands;

namespace Questline.Engine.Handlers;

public class GetCommandHandler : ICommandHandler<GetCommand>
{
    public CommandResult Execute(GameState state, GetCommand command)
    {
        var room = state.World.GetRoom(state.Player.Location);
        var item = room.Items.FindByName(command.ItemName);

        if (item is null)
        {
            return new ErrorResult($"There is no '{command.ItemName}' here.");
        }

        room.Items.Remove(item);
        state.Player.Inventory.Add(item);

        return new ItemPickedUpResult(item.Name);
    }
}
```

**DropCommandHandler:**

```csharp
using Questline.Domain;
using Questline.Engine.Commands;

namespace Questline.Engine.Handlers;

public class DropCommandHandler : ICommandHandler<DropCommand>
{
    public CommandResult Execute(GameState state, DropCommand command)
    {
        var item = state.Player.Inventory.FindByName(command.ItemName);

        if (item is null)
        {
            return new ErrorResult($"You are not carrying '{command.ItemName}'.");
        }

        state.Player.Inventory.Remove(item);
        var room = state.World.GetRoom(state.Player.Location);
        room.Items.Add(item);

        return new ItemDroppedResult(item.Name);
    }
}
```

**InventoryCommandHandler:**

```csharp
using Questline.Domain;
using Questline.Engine.Commands;

namespace Questline.Engine.Handlers;

public class InventoryCommandHandler : ICommandHandler<InventoryCommand>
{
    public CommandResult Execute(GameState state, InventoryCommand command)
    {
        var items = state.Player.Inventory.Items
            .Select(i => i.Name)
            .ToList();

        return new InventoryResult(items);
    }
}
```

### Updated Existing Handlers

**LookCommandHandler** — include room items:

```csharp
public CommandResult Execute(GameState state, LookCommand command)
{
    var room = state.World.GetRoom(state.Player.Location);
    var exits = room.Exits.Keys.Select(d => d.ToString()).ToList();
    var items = room.Items.Items.Select(i => i.Name).ToList();
    return new LookResult(room.Name, room.Description, exits, items);
}
```

**GoCommandHandler** — include destination room items:

```csharp
var newRoom = state.World.GetRoom(destinationId);
var exits = newRoom.Exits.Keys.Select(d => d.ToString()).ToList();
var items = newRoom.Items.Items.Select(i => i.Name).ToList();
return new MovedResult(newRoom.Name, newRoom.Description, exits, items);
```

### Dispatcher Registration

Register in `Program.cs` using factory functions. Use `string.Join(" ", args)` to support multi-word item names:

```csharp
dispatcher.Register(["get", "take"], new GetCommandHandler(), args =>
    args.Length == 0 ? null : new GetCommand(string.Join(" ", args)));

dispatcher.Register(["drop"], new DropCommandHandler(), args =>
    args.Length == 0 ? null : new DropCommand(string.Join(" ", args)));

dispatcher.Register(["inventory", "inv", "i"], new InventoryCommandHandler(),
    _ => new InventoryCommand());
```

**Note on no-argument errors:** When `args` is empty, the factory returns `null`, which causes the dispatcher to return `ErrorResult("Invalid command arguments.")`. Consider whether a more specific message (e.g., "Get what?" / "Drop what?") is needed. If so, handle empty args in the handler rather than the factory.

### Multi-word Item Names

The parser splits on spaces, so `"get brass lamp"` produces `args: ["brass", "lamp"]`. The factory function must rejoin them:

```csharp
args.Length == 0 ? null : new GetCommand(string.Join(" ", args))
```

### Test Approach

**Inventory Container Tests:**

```csharp
[Fact]
public void Add_ThenFindByName_ReturnsItem()
{
    var inventory = new Inventory();
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };

    inventory.Add(lamp);

    inventory.FindByName("brass lamp").ShouldBe(lamp);
}

[Fact]
public void FindByName_IsCaseInsensitive()
{
    var inventory = new Inventory();
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    inventory.Add(lamp);

    inventory.FindByName("BRASS LAMP").ShouldBe(lamp);
}

[Fact]
public void FindByName_WhenNotFound_ReturnsNull()
{
    var inventory = new Inventory();

    inventory.FindByName("sword").ShouldBeNull();
}

[Fact]
public void Remove_WhenItemExists_ReturnsTrueAndRemovesItem()
{
    var inventory = new Inventory();
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    inventory.Add(lamp);

    inventory.Remove(lamp).ShouldBeTrue();
    inventory.IsEmpty.ShouldBeTrue();
}

[Fact]
public void IsEmpty_WhenNoItems_ReturnsTrue()
{
    var inventory = new Inventory();

    inventory.IsEmpty.ShouldBeTrue();
}

[Fact]
public void Items_ReturnsReadonlyCollection()
{
    var inventory = new Inventory();
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    inventory.Add(lamp);

    inventory.Items.ShouldContain(lamp);
}
```

**Get Command Tests:**

```csharp
[Fact]
public void Get_WhenItemInRoom_AddsToInventoryAndRemovesFromRoom()
{
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    var handler = new GetCommandHandler();

    var result = handler.Execute(state, new GetCommand("brass lamp"));

    result.ShouldBeOfType<ItemPickedUpResult>();
    state.Player.Inventory.Items.ShouldContain(lamp);
    world.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBeNull();
}

[Fact]
public void Get_WhenItemNotInRoom_ReturnsError()
{
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.")
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    var handler = new GetCommandHandler();

    var result = handler.Execute(state, new GetCommand("lamp"));

    result.ShouldBeOfType<ErrorResult>();
    result.Success.ShouldBeFalse();
}

[Fact]
public void Get_IsCaseInsensitive()
{
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    var handler = new GetCommandHandler();

    var result = handler.Execute(state, new GetCommand("BRASS LAMP"));

    result.ShouldBeOfType<ItemPickedUpResult>();
}
```

**Drop Command Tests:**

```csharp
[Fact]
public void Drop_WhenItemInInventory_PlacesInRoomAndRemovesFromInventory()
{
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.")
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    state.Player.Inventory.Add(lamp);
    var handler = new DropCommandHandler();

    var result = handler.Execute(state, new DropCommand("brass lamp"));

    result.ShouldBeOfType<ItemDroppedResult>();
    state.Player.Inventory.IsEmpty.ShouldBeTrue();
    world.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBe(lamp);
}

[Fact]
public void Drop_WhenItemNotInInventory_ReturnsError()
{
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.")
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    var handler = new DropCommandHandler();

    var result = handler.Execute(state, new DropCommand("lamp"));

    result.ShouldBeOfType<ErrorResult>();
    result.Success.ShouldBeFalse();
}
```

**Inventory Command Tests:**

```csharp
[Fact]
public void Inventory_WhenCarryingItems_ListsThem()
{
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    var key = new Item { Id = "key", Name = "rusty key", Description = "A rusty iron key." };
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.")
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    state.Player.Inventory.Add(lamp);
    state.Player.Inventory.Add(key);
    var handler = new InventoryCommandHandler();

    var result = handler.Execute(state, new InventoryCommand());

    var inventoryResult = result.ShouldBeOfType<InventoryResult>();
    inventoryResult.Items.ShouldContain("brass lamp");
    inventoryResult.Items.ShouldContain("rusty key");
}

[Fact]
public void Inventory_WhenEmpty_ShowsMessage()
{
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.")
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    var handler = new InventoryCommandHandler();

    var result = handler.Execute(state, new InventoryCommand());

    result.Message.ShouldContain("not carrying anything");
}
```

**Look Enhancement Tests:**

```csharp
[Fact]
public void Look_WhenRoomHasItems_IncludesThemInResult()
{
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    var handler = new LookCommandHandler();

    var result = handler.Execute(state, new LookCommand());

    var lookResult = result.ShouldBeOfType<LookResult>();
    lookResult.Items.ShouldContain("brass lamp");
    lookResult.Message.ShouldContain("You can see");
}

[Fact]
public void Look_WhenRoomHasNoItems_DoesNotShowItemsLine()
{
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.")
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    var handler = new LookCommandHandler();

    var result = handler.Execute(state, new LookCommand());

    result.Message.ShouldNotContain("You can see");
}
```

**WorldBuilder Tests:**

```csharp
[Fact]
public void WithItem_AddsItemToRoom()
{
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
        .Build();

    var room = world.GetRoom("cellar");
    room.Items.FindByName("brass lamp").ShouldBe(lamp);
}
```

**Integration Test:**

```csharp
[Fact]
public void GetThenDrop_MovesItemFromRoomToInventoryAndBack()
{
    var lamp = new Item { Id = "lamp", Name = "brass lamp", Description = "A shiny brass lamp." };
    var world = new WorldBuilder()
        .WithRoom("cellar", "Cellar", "A damp cellar.", r => r.WithItem(lamp))
        .Build();
    var state = new GameState(world, new Player { Id = "player1", Location = "cellar" });
    var getHandler = new GetCommandHandler();
    var dropHandler = new DropCommandHandler();

    getHandler.Execute(state, new GetCommand("brass lamp"));
    state.Player.Inventory.Items.ShouldContain(lamp);
    world.GetRoom("cellar").Items.IsEmpty.ShouldBeTrue();

    dropHandler.Execute(state, new DropCommand("brass lamp"));
    state.Player.Inventory.IsEmpty.ShouldBeTrue();
    world.GetRoom("cellar").Items.FindByName("brass lamp").ShouldBe(lamp);
}
```

## Dependencies

- Phase 0.1: Project Scaffolding — requires Room, Player, GameState, World, WorldBuilder, Parser, CommandDispatcher, and the command handler pattern to be in place

## Out of Scope

- Partial or fuzzy item name matching (exact case-insensitive only)
- Item weight or encumbrance
- Item tags or properties/stats
- Pickup restrictions (items that can't be taken)
- Item stacking or quantities
- `examine <item>` command (deferred to later milestone)
- Container items (chest containing items)
- Using items (Phase 0.4)
- Loading items from content files (Phase 0.3)
- Party inventory (Phase 2)

## Decisions Made

_Record significant implementation decisions here as work progresses._
