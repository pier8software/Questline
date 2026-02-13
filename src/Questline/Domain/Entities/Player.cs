using Questline.Domain.Shared;

namespace Questline.Domain.Entities;

public class Player
{
    public required string Id { get; init; }
    public required string Location { get; set; }
    public Inventory Inventory { get; init; } = new();
}
