using Questline.Domain.Characters.Entity;
using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Persistence.Rooms;
using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Playthroughs;

public class PlaythroughMapper : IPersistenceMapper<Playthrough, PlaythroughDocument>
{
    public Playthrough From(PlaythroughDocument document) => new()
    {
        Id             = document.Id,
        AdventureId    = document.AdventureId,
        StartingRoomId = document.StartingRoomId,
        CharacterName  = document.CharacterName,
        Race           = Enum.Parse<Race>(document.Race),
        Class          = Enum.Parse<CharacterClass>(document.Class),
        Level          = document.Level,
        Experience     = document.Experience,
        AbilityScores = new AbilityScores(
            new AbilityScore(document.AbilityScores.Strength),
            new AbilityScore(document.AbilityScores.Intelligence),
            new AbilityScore(document.AbilityScores.Wisdom),
            new AbilityScore(document.AbilityScores.Dexterity),
            new AbilityScore(document.AbilityScores.Constitution),
            new AbilityScore(document.AbilityScores.Charisma)),
        HitPoints = new HitPoints(
            document.HitPoints.MaxHitPoints,
            document.HitPoints.CurrentHitPoints),
        Location = document.Location,
        Inventory = document.Inventory.Select(i => new Item
        {
            Id          = i.Id,
            Name        = i.Name,
            Description = i.Description
        }).ToList(),
        UnlockedBarriers = document.UnlockedBarriers.ToHashSet(),
        RoomItems = document.RoomItems.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(i => new Item
            {
                Id          = i.Id,
                Name        = i.Name,
                Description = i.Description
            }).ToList())
    };

    public PlaythroughDocument To(Playthrough entity) => new()
    {
        Id             = entity.Id,
        AdventureId    = entity.AdventureId,
        StartingRoomId = entity.StartingRoomId,
        CharacterName  = entity.CharacterName,
        Race           = entity.Race.ToString(),
        Class          = entity.Class.ToString(),
        Level          = entity.Level,
        Experience     = entity.Experience,
        AbilityScores = new AbilityScoresDocument
        {
            Strength     = entity.AbilityScores.Strength.Score,
            Intelligence = entity.AbilityScores.Intelligence.Score,
            Wisdom       = entity.AbilityScores.Wisdom.Score,
            Dexterity    = entity.AbilityScores.Dexterity.Score,
            Constitution = entity.AbilityScores.Constitution.Score,
            Charisma     = entity.AbilityScores.Charisma.Score
        },
        HitPoints = new HitPointsDocument
        {
            MaxHitPoints     = entity.HitPoints.MaxHitPoints,
            CurrentHitPoints = entity.HitPoints.CurrentHitPoints
        },
        Location         = entity.Location,
        Inventory        = entity.Inventory.Select(ToItemDocument).ToList(),
        UnlockedBarriers = entity.UnlockedBarriers.ToList(),
        RoomItems = entity.RoomItems.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(ToItemDocument).ToList())
    };

    private static ItemDocument ToItemDocument(Item item) => new()
    {
        Id          = item.Id,
        Name        = item.Name,
        Description = item.Description
    };
}
