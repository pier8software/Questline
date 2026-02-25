using Questline.Domain.Shared.Data;
using Questline.Framework.Domain;

namespace Questline.Domain.Playthroughs.Entity;

public class Playthrough : DomainEntity
{
    public string         PlayerId    { get; set; } = null!;
    public string         CharacterId { get; set; } = null!;
    public string         AdventureId { get; set; } = null!;
    public GameState      State       { get; set; } = null!;
    public DateTimeOffset StartedAt   { get; set; }
    public DateTimeOffset LastSavedAt { get; set; }
}
