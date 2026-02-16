namespace Questline.Domain.Shared.Entity;

public class Item
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}

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
