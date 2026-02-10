namespace Questline.Domain;

public class Adventure
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required World World { get; init; }
    public required string StartingRoomId { get; init; }
}
