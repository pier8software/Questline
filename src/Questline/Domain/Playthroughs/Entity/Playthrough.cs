using Questline.Domain.Playthroughs.Persistence;
using Questline.Domain.Shared.Data;
using Questline.Framework.Domain;

namespace Questline.Domain.Playthroughs.Entity;

public class Playthrough : DomainEntity
{
    public string         PlayerId    { get; init; }        = null!;
    public string         CharacterId { get; private set; } = null!;
    public string         AdventureId { get; init; }        = null!;
    public GameState      State       { get; init; }        = null!;
    public DateTimeOffset StartedAt   { get; init; }
    public DateTimeOffset LastSavedAt { get; init; }

    public static Playthrough From(PlaythroughDocument document) =>
        new()
        {
            Id          = document.Id,
            PlayerId    = document.PlayerId,
            CharacterId = document.CharacterId,
            AdventureId = document.AdventureId,
            //State       = document.State,
            StartedAt   = document.StartedAt,
            LastSavedAt = document.UpdatedAt
        };
}
