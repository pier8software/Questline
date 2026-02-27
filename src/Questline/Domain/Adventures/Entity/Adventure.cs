using Questline.Framework.Domain;

namespace Questline.Domain.Adventures.Entity;

public class Adventure : DomainEntity
{
    public required string Name           { get; init; }
    public required string Description    { get; init; }
    public required string StartingRoomId { get; init; }
}
