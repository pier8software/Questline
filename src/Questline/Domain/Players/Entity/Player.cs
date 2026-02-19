using Questline.Domain.Characters.Entity;
using Questline.Domain.Shared.Entity;

namespace Questline.Domain.Players.Entity;

public class Player
{
    public required string Id { get; init; }
    public required Character Character { get; init; }
    public required string Location { get; set; }
    public Inventory Inventory { get; init; } = new();
}
