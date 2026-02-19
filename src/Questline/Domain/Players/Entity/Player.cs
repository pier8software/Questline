using Questline.Domain.Characters.Entity;

namespace Questline.Domain.Players.Entity;

public class Player
{
    public required string Id { get; init; }
    public required Character Character { get; init; }
}
