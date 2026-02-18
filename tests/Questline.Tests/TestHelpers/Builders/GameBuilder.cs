using Questline.Domain.Rooms.Entity;

namespace Questline.Tests.TestHelpers.Builders;

public class GameBuilder
{
    private readonly Dictionary<string, Room> _rooms = new();

    public GameBuilder WithRoom(string id, string name, string description, Action<RoomBuilder>? configure = null)
    {
        var builder = new RoomBuilder(id, name, description);
        configure?.Invoke(builder);
        _rooms[id] = builder.Build();
        return this;
    }

    public Dictionary<string, Room> Build() => _rooms;
}
