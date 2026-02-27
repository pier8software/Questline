using Questline.Domain.Shared.Entity;
using Questline.Framework.Domain;

namespace Questline.Domain.Rooms.Entity;

public class Room : DomainEntity
{
    public required string Name        { get; init; }
    public required string Description { get; init; }

    public IReadOnlyDictionary<Direction, Exit> Exits { get; init; } =
        new Dictionary<Direction, Exit>();

    public IReadOnlyList<Item> Items { get; init; } = [];

    public IReadOnlyList<Feature> Features { get; init; } = [];
}

public record Exit(string Destination, Barrier? Barrier = null);

public enum Direction
{
    Invalid = 0,
    North   = 1,
    South   = 2,
    East    = 3,
    West    = 4,
    Up      = 5,
    Down    = 6
}
