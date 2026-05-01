using Questline.Domain.Characters.Entity;
using Questline.Domain.Parties.Entity;
using Questline.Domain.Playthroughs.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Engine.Persistence.Rooms;
using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Playthroughs;

public class PlaythroughMapper : IPersistenceMapper<Playthrough, PlaythroughDocument>
{
    public Playthrough From(PlaythroughDocument document)
    {
        var playthrough = new Playthrough
        {
            Id               = document.Id,
            Username         = document.Username,
            AdventureId      = document.AdventureId,
            StartingRoomId   = document.StartingRoomId,
            Location         = document.Location,
            Party            = MapParty(document.Party),
            UnlockedBarriers = document.UnlockedBarriers.ToHashSet(),
            RoomItems        = document.RoomItems.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(MapItem).ToList())
        };

        playthrough.RestoreTurns(document.Turns);
        return playthrough;
    }

    public PlaythroughDocument To(Playthrough entity) => new()
    {
        Id               = entity.Id,
        Username         = entity.Username,
        AdventureId      = entity.AdventureId,
        StartingRoomId   = entity.StartingRoomId,
        Location         = entity.Location,
        Turns            = entity.Turns,
        Party            = new PartyDocument
        {
            Members = entity.Party.Members.Select(MapCharacterToDoc).ToList()
        },
        UnlockedBarriers = entity.UnlockedBarriers.ToList(),
        RoomItems        = entity.RoomItems.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(MapItemToDoc).ToList())
    };

    private static Party MapParty(PartyDocument doc) =>
        new(
            id: Guid.NewGuid().ToString(),
            members: doc.Members.Select(MapCharacterFromDoc).ToList());

    private static Character MapCharacterFromDoc(CharacterDocument doc)
    {
        var character = Character.Create(
            id: doc.Id,
            name: doc.Name,
            race: Enum.Parse<Race>(doc.Race),
            characterClass: doc.Class is null ? null : Enum.Parse<CharacterClass>(doc.Class),
            hitPoints: new HitPoints(max: doc.HitPoints.MaxHitPoints, current: doc.HitPoints.CurrentHitPoints),
            abilityScores: new AbilityScores(
                new AbilityScore(doc.AbilityScores.Strength),
                new AbilityScore(doc.AbilityScores.Intelligence),
                new AbilityScore(doc.AbilityScores.Wisdom),
                new AbilityScore(doc.AbilityScores.Dexterity),
                new AbilityScore(doc.AbilityScores.Constitution),
                new AbilityScore(doc.AbilityScores.Charisma)),
            occupation: doc.Occupation,
            level: doc.Level);

        foreach (var item in doc.Inventory)
        {
            character.AddInventoryItem(MapItem(item));
        }

        return character;
    }

    private static CharacterDocument MapCharacterToDoc(Character character) => new()
    {
        Id         = character.Id,
        Name       = character.Name,
        Race       = character.Race.ToString(),
        Class      = character.Class?.ToString(),
        Level      = character.Level,
        Experience = character.Experience,
        Occupation = character.Occupation,
        AbilityScores = new AbilityScoresDocument
        {
            Strength     = character.AbilityScores.Strength.Score,
            Intelligence = character.AbilityScores.Intelligence.Score,
            Wisdom       = character.AbilityScores.Wisdom.Score,
            Dexterity    = character.AbilityScores.Dexterity.Score,
            Constitution = character.AbilityScores.Constitution.Score,
            Charisma     = character.AbilityScores.Charisma.Score
        },
        HitPoints = new HitPointsDocument
        {
            MaxHitPoints     = character.HitPoints.Max,
            CurrentHitPoints = character.HitPoints.Current
        },
        Inventory = character.Inventory.Select(MapItemToDoc).ToList()
    };

    private static Item MapItem(ItemDocument doc) => new()
    {
        Id          = doc.Id,
        Name        = doc.Name,
        Description = doc.Description
    };

    private static ItemDocument MapItemToDoc(Item item) => new()
    {
        Id          = item.Id,
        Name        = item.Name,
        Description = item.Description
    };
}
