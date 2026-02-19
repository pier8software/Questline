using Questline.Domain.Characters.Entity;
using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.TestHelpers.Builders;

public class GameBuilder
{
    private static readonly Character DefaultCharacter = new("TestHero", Race.Human, CharacterClass.Fighter);

    private readonly Dictionary<string, Barrier> _barriers = new();
    private readonly Dictionary<string, Room> _rooms = new();
    private Character _character = DefaultCharacter;

    public GameBuilder WithRoom(string id, string name, string description, Action<RoomBuilder>? configure = null)
    {
        var builder = new RoomBuilder(id, name, description);
        configure?.Invoke(builder);
        _rooms[id] = builder.Build();
        return this;
    }

    public GameBuilder WithBarrier(Barrier barrier)
    {
        _barriers[barrier.Id] = barrier;
        return this;
    }

    public GameBuilder WithCharacter(Character character)
    {
        _character = character;
        return this;
    }

    public Dictionary<string, Room> Build() => _rooms;

    public GameState BuildState(string playerId, string startLocation) =>
        new(_rooms, new Player { Id = playerId, Character = _character, Location = startLocation }, _barriers);
}
