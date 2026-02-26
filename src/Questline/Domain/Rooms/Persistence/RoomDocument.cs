using Questline.Framework.Persistence;

namespace Questline.Domain.Rooms.Persistence;

public class RoomDocument : Document
{
    public required string Name        { get; init; }
    public required string Description { get; init; }
}

public class BarrierDocument : Document
{
    public required string Name        { get; init; }
    public required string Description { get; init; }
}
