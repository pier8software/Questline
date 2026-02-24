using System.Collections.Immutable;

namespace Questline.Domain.Shared.Entity;

public class Item
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public record Inventory
{
    private readonly ImmutableList<Item> _items;

    public Inventory() : this(ImmutableList<Item>.Empty)
    {
    }

    private Inventory(ImmutableList<Item> items)
    {
        _items = items;
    }

    public IReadOnlyList<Item> Items => _items;
    public bool IsEmpty => _items.IsEmpty;

    public Inventory Add(Item item) => new(_items.Add(item));

    public Inventory Remove(Item item) => new(_items.Remove(item));

    public Item? FindByName(string name) =>
        _items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public bool Contains(Item item) => _items.Contains(item);
}
