using Questline.Domain.Shared.Entity;

namespace Questline.Domain.Rooms.Entity;

public class Room
{
    private Dictionary<Direction, Exit> _exits = new();
    private List<Item> _items = [];
    private List<Feature> _features = [];

    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    public IReadOnlyDictionary<Direction, Exit> Exits
    {
        get => _exits;
        init => _exits = new Dictionary<Direction, Exit>(value);
    }

    public IReadOnlyList<Item> Items
    {
        get => _items;
        init => _items = new List<Item>(value);
    }

    public IReadOnlyList<Feature> Features
    {
        get => _features;
        init => _features = new List<Feature>(value);
    }

    public void AddItem(Item item) => _items.Add(item);

    public void RemoveItem(Item item) => _items.Remove(item);

    public Item? FindItemByName(string name) =>
        _items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}

public record Exit(string Destination, string? BarrierId = null);

public enum Direction
{
    Invalid = 0,
    North = 1,
    South = 2,
    East = 3,
    West = 4,
    Up = 5,
    Down = 6
}
