using MongoDB.Bson.Serialization.Attributes;

namespace Questline.Framework.Persistence;

public record GameStateDocument
{
    [BsonId] public required string Id { get; init; }

    public required string AdventureId { get; init; }
    public required string Version { get; init; }
    public required PlayerDocument Player { get; init; }
    public required List<RoomDocument> Rooms { get; init; }
    public required List<BarrierDocument> Barriers { get; init; }
}

public record PlayerDocument
{
    public required string Id { get; init; }
    public required CharacterDocument Character { get; init; }
}

public record CharacterDocument
{
    public required string Name { get; init; }
    public required string Race { get; init; }
    public required string Class { get; init; }
    public required int Level { get; init; }
    public required int Experience { get; init; }
    public required AbilityScoresDocument AbilityScores { get; init; }
    public required HitPointsDocument HitPoints { get; init; }
    public required string Location { get; init; }
    public required List<ItemDocument> Inventory { get; init; }
}

public record AbilityScoresDocument
{
    public required int Strength { get; init; }
    public required int Intelligence { get; init; }
    public required int Wisdom { get; init; }
    public required int Dexterity { get; init; }
    public required int Constitution { get; init; }
    public required int Charisma { get; init; }
}

public record HitPointsDocument
{
    public required int MaxHitPoints { get; init; }
    public required int CurrentHitPoints { get; init; }
}

public record ItemDocument
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public record RoomDocument
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required List<ExitDocument> Exits { get; init; }
    public required List<ItemDocument> Items { get; init; }
    public required List<FeatureDocument> Features { get; init; }
}

public record ExitDocument
{
    public required string Direction { get; init; }
    public required string Destination { get; init; }
    public string? BarrierId { get; init; }
}

public record FeatureDocument
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string[] Keywords { get; init; }
    public required string Description { get; init; }
}

public record BarrierDocument
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string BlockedMessage { get; init; }
    public required string UnlockItemId { get; init; }
    public required string UnlockMessage { get; init; }
    public required bool IsUnlocked { get; init; }
}
