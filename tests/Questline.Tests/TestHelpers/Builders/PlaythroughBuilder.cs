using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Domain.Playthroughs.Entity;

namespace Questline.Tests.TestHelpers.Builders;

public class PlaythroughBuilder
{
    private string          _id             = "test-playthrough";
    private string          _username       = "test-user";
    private string          _adventureId    = "test-adventure";
    private string          _startingRoomId = "start";
    private string          _location       = "start";
    private List<Character> _members        = [];

    public static PlaythroughBuilder New() => new();

    public PlaythroughBuilder WithId(string id)             { _id = id; return this; }
    public PlaythroughBuilder WithUsername(string u)         { _username = u; return this; }
    public PlaythroughBuilder WithAdventureId(string id)    { _adventureId = id; return this; }
    public PlaythroughBuilder WithStartingRoomId(string id) { _startingRoomId = id; _location = id; return this; }
    public PlaythroughBuilder WithLocation(string id)        { _location = id; return this; }
    public PlaythroughBuilder WithCharacter(Character c)     { _members.Add(c); return this; }

    /// <summary>Convenience overload — sets the leader's name via CharacterBuilder.
    /// Preserves API used in existing tests that called WithCharacterName.</summary>
    public PlaythroughBuilder WithCharacterName(string name)
    {
        _members = [CharacterBuilder.New().WithName(name).Build()];
        return this;
    }

    public PlaythroughBuilder WithInventoryItem(Questline.Domain.Shared.Entity.Item item)
    {
        EnsureLeader();
        _members[0].AddInventoryItem(item);
        return this;
    }

    public PlaythroughBuilder WithUnlockedBarrier(string barrierId)
    {
        // UnlockedBarriers are set post-build via init.
        // We collect them and apply in Build().
        _unlockedBarriers.Add(barrierId);
        return this;
    }

    private readonly HashSet<string> _unlockedBarriers = [];

    private void EnsureLeader()
    {
        if (_members.Count == 0)
            _members.Add(CharacterBuilder.New().Build());
    }

    public Playthrough Build()
    {
        var members = _members.Count > 0 ? _members : [CharacterBuilder.New().Build()];
        var party   = new Party(id: Guid.NewGuid().ToString(), members: members);

        return new Playthrough
        {
            Id               = _id,
            Username         = _username,
            AdventureId      = _adventureId,
            StartingRoomId   = _startingRoomId,
            Location         = _location,
            Party            = party,
            UnlockedBarriers = _unlockedBarriers
        };
    }
}
