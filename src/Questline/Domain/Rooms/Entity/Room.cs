using Questline.Domain.Shared.Entity;

namespace Questline.Domain.Rooms.Entity;

public class Room
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public Dictionary<Direction, Exit> Exits { get; init; } = new();
    public Inventory Items { get; init; } = new();
    public List<Feature> Features { get; init; } = new();
}

public record Exit(string Destination, string? BarrierId = null);

public enum Direction
{
    Invalid = 0,
    North = 1,
    South = 2,
    East = 3,
    West = 4,
    Up = 5,
    Down = 6
}
