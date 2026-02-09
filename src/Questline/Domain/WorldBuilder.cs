namespace Questline.Domain;

public class WorldBuilder
{
    private readonly Dictionary<string, Room> _rooms = new();

    public WorldBuilder WithRoom(string id, string name, string description, Action<RoomBuilder>? configure = null)
    {
        var builder = new RoomBuilder(id, name, description);
        configure?.Invoke(builder);
        _rooms[id] = builder.Build();
        return this;
    }

    public World Build() => new(_rooms);
}

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
