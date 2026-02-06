# Phase 0.2: Items and Inventory

## Status

Not Started

## Objective

Add items to the world that players can pick up, carry, and drop. Lay the groundwork for puzzle mechanics and gear.

## Acceptance Criteria

### Item Model
- [ ] Items exist as entities with ID, name, and description
- [ ] Items can be placed in rooms
- [ ] Items can be held in player inventory
- [ ] `look` command now lists items visible in the current room

### Commands
- [ ] `get <item>` — picks up item from current room and adds to inventory
- [ ] `get <item>` — displays error if item not present in room
- [ ] `drop <item>` — removes item from inventory and places in current room
- [ ] `drop <item>` — displays error if item not in inventory
- [ ] `inventory` (or `i`) — lists items currently carried

### Edge Cases
- [ ] Item names are matched case-insensitively
- [ ] Partial matches work if unambiguous (e.g., "get lamp" matches "brass lamp")
- [ ] Ambiguous matches prompt for clarification or list options

## Implementation Notes

### Domain Entities

```csharp
public class Item
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    // Future: Weight, Properties, etc.
}
```

### Inventory as Container

Model inventory as a generic container for future reuse:

```csharp
public class Inventory
{
    private readonly List<Item> _items = new();
    
    public IReadOnlyList<Item> Items => _items;
    public void Add(Item item) => _items.Add(item);
    public bool Remove(Item item) => _items.Remove(item);
    public Item? Find(string name) => // matching logic
}
```

Both `Player.Inventory` and `Room.Items` can use this abstraction. Party inventory (Phase 2+) will use the same pattern.

### Matching Logic

For item name matching:

1. Exact match (case-insensitive)
2. Starts-with match if unambiguous
3. Contains match if unambiguous
4. If multiple matches, return error with options

### Room Description Update

`look` should now output:

```
The Dusty Cellar
A damp stone cellar with cobwebs in every corner. A faint light filters through a grate above.

You can see: a brass lamp, a rusty key

Exits: north, up
```

### Test Approach

```csharp
[Fact]
public void Get_WhenItemInRoom_AddsToInventory()
{
    // Arrange: room with lamp
    // Act: execute "get lamp"
    // Assert: lamp in player inventory, lamp not in room
}

[Fact]
public void Get_WhenItemNotInRoom_ReturnsError()
{
    // Arrange: empty room
    // Act: execute "get lamp"
    // Assert: error result, inventory unchanged
}

[Fact]
public void Drop_WhenItemInInventory_PlacesInRoom()
{
    // Arrange: player holding lamp
    // Act: execute "drop lamp"
    // Assert: lamp in room, lamp not in inventory
}

[Fact]
public void Inventory_ListsCarriedItems()
{
    // Arrange: player holding lamp and key
    // Act: execute "inventory"
    // Assert: result lists both items
}
```

## Out of Scope

- Using items (Phase 0.4)
- Item properties/stats (Phase 1)
- Party inventory (Phase 2)
- Loading items from content files (Phase 0.3)
