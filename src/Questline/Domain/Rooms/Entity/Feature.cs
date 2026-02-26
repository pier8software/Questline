namespace Questline.Domain.Rooms.Entity;

public class Feature
{
    public required string   Id          { get; init; }
    public required string   Name        { get; init; }
    public required string[] Keywords    { get; init; }
    public required string   Description { get; init; }
}
