namespace Questline.Domain.Rooms.Entity;

public record Barrier
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string BlockedMessage { get; init; }
    public required string UnlockItemId { get; init; }
    public required string UnlockMessage { get; init; }
    public bool IsUnlocked { get; init; }

    public Barrier Unlock() => this with { IsUnlocked = true };
}
