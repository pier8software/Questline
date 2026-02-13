using Questline.Domain.Rooms.Entity;

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
