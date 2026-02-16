using Questline.Domain.Shared.Entity;

namespace Questline.Domain.Rooms.Entity;

public class Room
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public Dictionary<Direction, Exit> Exits { get; init; } = new();
    public Inventory Items { get; init; } = new();
}

public record Exit(string Destination, string? BarrierId = null);

public enum Direction
{
    North,
    South,
    East,
    West,
    Up,
    Down
}
