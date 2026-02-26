using Questline.Domain.Characters.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Engine.Content;
using Questline.Engine.Core;
using Questline.Domain.Players.Entity;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Tests.TestHelpers.Builders;

public class GameBuilder
{
    private static readonly HitPoints DefaultHitPoints = new(8, 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));

    private static readonly Func<string, Character> DefaultCharacterFactory =
        location => Character.Create("test-character-id", "TestHero", Race.Human, CharacterClass.Fighter,
            DefaultHitPoints, DefaultAbilityScores, location);

    private readonly Dictionary<string, Barrier> _barriers = new();
    private readonly Dictionary<string, Room>    _rooms    = new();
    private          Func<string, Character>?    _characterFactory;

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
        _characterFactory = _ => character;
        return this;
    }

    public Dictionary<string, Room> Build() => _rooms;

    public AdventureContent BuildWorldContent(string startingRoomId) =>
        new(_rooms, _barriers, startingRoomId);

    public GameState BuildState(string playerId, string startLocation)
    {
        var character = _characterFactory is not null
            ? _characterFactory(startLocation)
            : DefaultCharacterFactory(startLocation);

        character.MoveTo(startLocation);

        var adventure = new AdventureContent(_rooms, _barriers, startLocation);
        var player    = Player.Create(playerId, playerId, playerId);

        return new GameState
        {
            Phase     = GamePhase.Playing,
            Player    = player,
            Adventure = adventure,
            Character = character
        };
    }
}
