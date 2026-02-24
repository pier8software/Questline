using MongoDB.Driver;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Framework.Persistence;

public class MongoGameStateRepository(IMongoCollection<GameStateDocument> collection, string version)
    : IGameStateRepository
{
    public void Save(GameState state)
    {
        var document = ToDocument(state);
        var filter = Builders<GameStateDocument>.Filter.Eq(d => d.Id, document.Id);
        collection.ReplaceOne(filter, document, new ReplaceOptions { IsUpsert = true });
    }

    private GameStateDocument ToDocument(GameState state)
    {
        var player = state.Player;
        var character = player.Character;

        return new GameStateDocument
        {
            Id = player.Id,
            AdventureId = state.AdventureId,
            Version = version,
            Player = new PlayerDocument
            {
                Id = player.Id,
                Character = new CharacterDocument
                {
                    Name = character.Name,
                    Race = character.Race.ToString(),
                    Class = character.Class.ToString(),
                    Level = character.Level,
                    Experience = character.Experience,
                    AbilityScores = new AbilityScoresDocument
                    {
                        Strength = character.AbilityScores.Strength.Score,
                        Intelligence = character.AbilityScores.Intelligence.Score,
                        Wisdom = character.AbilityScores.Wisdom.Score,
                        Dexterity = character.AbilityScores.Dexterity.Score,
                        Constitution = character.AbilityScores.Constitution.Score,
                        Charisma = character.AbilityScores.Charisma.Score
                    },
                    HitPoints = new HitPointsDocument
                    {
                        MaxHitPoints = character.HitPoints.MaxHitPoints,
                        CurrentHitPoints = character.HitPoints.CurrentHitPoints
                    },
                    Location = character.Location,
                    Inventory = character.Inventory.Select(ToItemDocument).ToList()
                }
            },
            Rooms = state.Rooms.Select(ToRoomDocument).ToList(),
            Barriers = state.Barriers.Values.Select(ToBarrierDocument).ToList()
        };
    }

    private static ItemDocument ToItemDocument(Item item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description
    };

    private static RoomDocument ToRoomDocument(Room room) => new()
    {
        Id = room.Id,
        Name = room.Name,
        Description = room.Description,
        Exits = room.Exits.Select(e => new ExitDocument
        {
            Direction = e.Key.ToString(),
            Destination = e.Value.Destination,
            BarrierId = e.Value.BarrierId
        }).ToList(),
        Items = room.Items.Select(ToItemDocument).ToList(),
        Features = room.Features.Select(f => new FeatureDocument
        {
            Id = f.Id,
            Name = f.Name,
            Keywords = f.Keywords,
            Description = f.Description
        }).ToList()
    };

    private static BarrierDocument ToBarrierDocument(Barrier barrier) => new()
    {
        Id = barrier.Id,
        Name = barrier.Name,
        Description = barrier.Description,
        BlockedMessage = barrier.BlockedMessage,
        UnlockItemId = barrier.UnlockItemId,
        UnlockMessage = barrier.UnlockMessage,
        IsUnlocked = barrier.IsUnlocked
    };
}
