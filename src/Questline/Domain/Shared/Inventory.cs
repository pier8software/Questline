using Questline.Domain.Entities;

namespace Questline.Domain.Shared;

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
