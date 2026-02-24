using System.Collections.Immutable;
using Questline.Domain.Shared.Entity;

namespace Questline.Domain.Rooms.Entity;

public record Room
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public ImmutableDictionary<Direction, Exit> Exits { get; init; } = ImmutableDictionary<Direction, Exit>.Empty;
    public ImmutableList<Item> Items { get; init; } = ImmutableList<Item>.Empty;
    public ImmutableList<Feature> Features { get; init; } = ImmutableList<Feature>.Empty;

    public Room AddItem(Item item) => this with { Items = Items.Add(item) };

    public Room RemoveItem(Item item) => this with { Items = Items.Remove(item) };

    public Item? FindItemByName(string name) =>
        Items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
