using Questline.Domain.Adventures.Entity;
using Questline.Domain.Characters.Entity;
using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Core;
using Questline.Engine.Repositories;

namespace Questline.Tests.TestHelpers.Builders;

public class GameBuilder
{
    private static readonly HitPoints DefaultHitPoints = new(8, 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));

    private readonly Dictionary<string, Room> _rooms = new();

    private readonly string           _adventureId   = "test-adventure";
    private readonly string           _characterName = "TestHero";
    private readonly Race             _race          = Race.Human;
    private readonly CharacterClass   _class         = CharacterClass.Fighter;
    private readonly AbilityScores    _abilityScores = DefaultAbilityScores;
    private readonly HitPoints        _hitPoints     = DefaultHitPoints;
    private          List<Item>?      _inventoryItems;
    private          HashSet<string>? _unlockedBarriers;

    public GameBuilder WithRoom(string id, string name, string description, Action<RoomBuilder>? configure = null)
    {
        var builder = new RoomBuilder(id, name, description);
        configure?.Invoke(builder);
        _rooms[id] = builder.Build();
        return this;
    }

    public GameBuilder WithInventoryItem(Item item)
    {
        _inventoryItems ??= [];
        _inventoryItems.Add(item);
        return this;
    }

    public GameBuilder WithUnlockedBarrier(string barrierId)
    {
        _unlockedBarriers ??= [];
        _unlockedBarriers.Add(barrierId);
        return this;
    }

    public GameFixture Build(string startLocation)
    {
        var playthrough = new Playthrough
        {
            Id               = "test-playthrough",
            AdventureId      = _adventureId,
            StartingRoomId   = startLocation,
            CharacterName    = _characterName,
            Race             = _race,
            Class            = _class,
            AbilityScores    = _abilityScores,
            HitPoints        = _hitPoints,
            Location         = startLocation,
            Inventory        = _inventoryItems   ?? [],
            UnlockedBarriers = _unlockedBarriers ?? []
        };

        var playthroughRepo = new FakePlaythroughRepository(playthrough);
        var roomRepo        = new FakeRoomRepository(_rooms);
        var session         = new FakeGameSession(playthrough.Id);

        return new GameFixture(playthrough, playthroughRepo, roomRepo, session);
    }
}

public record GameFixture(
    Playthrough               Playthrough,
    FakePlaythroughRepository PlaythroughRepository,
    FakeRoomRepository        RoomRepository,
    FakeGameSession           Session);

public class FakePlaythroughRepository : IPlaythroughRepository
{
    private readonly Dictionary<string, Playthrough> _store = new();

    public FakePlaythroughRepository()
    {
    }

    public FakePlaythroughRepository(Playthrough playthrough)
    {
        _store[playthrough.Id] = playthrough;
    }

    public Task<Playthrough> GetById(string id) =>
        Task.FromResult(_store[id]);

    public Task Save(Playthrough playthrough)
    {
        _store[playthrough.Id] = playthrough;
        return Task.CompletedTask;
    }
}

public class FakeRoomRepository(Dictionary<string, Room> rooms) : IRoomRepository
{
    public Task<Room> GetById(string roomId) =>
        Task.FromResult(rooms[roomId]);
}

public class FakeAdventureRepository : IAdventureRepository
{
    private readonly Dictionary<string, Adventure> _store = new();

    public FakeAdventureRepository(Adventure adventure)
    {
        _store[adventure.Id] = adventure;
    }

    public Task<Adventure> GetById(string id) =>
        Task.FromResult(_store[id]);
}

public class FakeGameSession(string playthroughId) : IGameSession
{
    public string? PlaythroughId { get; private set; } = playthroughId;

    public void SetPlaythroughId(string id) => PlaythroughId = id;
}
