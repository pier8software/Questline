using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;

namespace Questline.Tests.TestHelpers.Builders;

public class RoomBuilder(string id, string name, string description)
{
    private readonly Dictionary<Direction, Exit> _exits = new();
    private readonly List<Item> _items = new();

    public RoomBuilder WithExit(Direction direction, string destinationId)
        => WithExit(direction, new Exit(destinationId));

    public RoomBuilder WithExit(Direction direction, Exit exit)
    {
        _exits[direction] = exit;
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
            Exits = new Dictionary<Direction, Exit>(_exits)
        };

        foreach (var item in _items)
        {
            room.Items.Add(item);
        }

        return room;
    }
}
