namespace Questline.Domain;

public class Room
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public Dictionary<Direction, Exit> Exits { get; init; } = new();
    public Inventory Items { get; init; } = new();
}
