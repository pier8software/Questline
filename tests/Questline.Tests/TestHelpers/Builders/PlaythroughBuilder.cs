using Questline.Domain.Characters.Entity;
using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Shared.Entity;
using TestStack.Dossier;

namespace Questline.Tests.TestHelpers.Builders;

public class PlaythroughBuilder : TestDataBuilder<Playthrough, PlaythroughBuilder>
{
    private static readonly HitPoints DefaultHitPoints = new(8, 8);

    private static readonly AbilityScores DefaultAbilityScores = new(
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10),
        new AbilityScore(10), new AbilityScore(10), new AbilityScore(10));

    private List<Item>?      _inventoryItems;
    private HashSet<string>? _unlockedBarriers;

    public PlaythroughBuilder()
    {
        Set(x => x.Id, "test-playthrough");
        Set(x => x.Username, "test-user");
        Set(x => x.AdventureId, "test-adventure");
        Set(x => x.StartingRoomId, "start");
        Set(x => x.CharacterName, "TestHero");
        Set(x => x.Race, Race.Human);
        Set(x => x.Class, CharacterClass.Fighter);
        Set(x => x.AbilityScores, DefaultAbilityScores);
        Set(x => x.HitPoints, DefaultHitPoints);
        Set(x => x.Location, "start");
    }

    public PlaythroughBuilder WithId(string id) =>
        Set(x => x.Id, id);

    public PlaythroughBuilder WithUsername(string username) =>
        Set(x => x.Username, username);

    public PlaythroughBuilder WithAdventureId(string adventureId) =>
        Set(x => x.AdventureId, adventureId);

    public PlaythroughBuilder WithStartingRoomId(string startingRoomId) =>
        Set(x => x.StartingRoomId, startingRoomId);

    public PlaythroughBuilder WithCharacterName(string characterName) =>
        Set(x => x.CharacterName, characterName);

    public PlaythroughBuilder WithRace(Race race) =>
        Set(x => x.Race, race);

    public PlaythroughBuilder WithClass(CharacterClass @class) =>
        Set(x => x.Class, @class);

    public PlaythroughBuilder WithAbilityScores(AbilityScores abilityScores) =>
        Set(x => x.AbilityScores, abilityScores);

    public PlaythroughBuilder WithHitPoints(HitPoints hitPoints) =>
        Set(x => x.HitPoints, hitPoints);

    public PlaythroughBuilder WithLocation(string location) =>
        Set(x => x.Location, location);

    public PlaythroughBuilder WithInventoryItem(Item item)
    {
        _inventoryItems ??= [];
        _inventoryItems.Add(item);
        return this;
    }

    public PlaythroughBuilder WithUnlockedBarrier(string barrierId)
    {
        _unlockedBarriers ??= [];
        _unlockedBarriers.Add(barrierId);
        return this;
    }

    protected override Playthrough BuildObject()
    {
        var location = Get(x => x.Location);
        return new Playthrough
        {
            Id               = Get(x => x.Id),
            Username         = Get(x => x.Username),
            AdventureId      = Get(x => x.AdventureId),
            StartingRoomId   = Get(x => x.StartingRoomId),
            CharacterName    = Get(x => x.CharacterName),
            Race             = Get(x => x.Race),
            Class            = Get(x => x.Class),
            AbilityScores    = Get(x => x.AbilityScores),
            HitPoints        = Get(x => x.HitPoints),
            Location         = location,
            Inventory        = _inventoryItems   ?? [],
            UnlockedBarriers = _unlockedBarriers ?? []
        };
    }
}
