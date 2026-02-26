using Questline.Domain.Adventures.Entity;
using Questline.Domain.Playthroughs.Persistence;
using Questline.Domain.Shared.Data;
using Questline.Engine.Content.Data;
using Questline.Framework.Domain;

namespace Questline.Domain.Playthroughs.Entity;

public class Playthrough : DomainEntity
{
    public string         PlayerId    { get; init; }        = null!;
    public string         CharacterId { get; private set; } = null!;
    public Adventure      Adventure   { get; init; }        = null!;
    public DateTimeOffset StartedAt   { get; init; }
    public DateTimeOffset LastSavedAt { get; init; }

    public static Playthrough From(PlaythroughDocument document) =>
        new()
        {
            Id          = document.Id,
            PlayerId    = document.PlayerId,
            CharacterId = document.CharacterId,
            Adventure   = document.Adventure,
            StartedAt   = document.StartedAt,
            LastSavedAt = document.UpdatedAt
        };

    public static Playthrough Create(string playerId, AdventureContent adventureContent)
    {
        return new Playthrough
        {
            PlayerId    = playerId,
            Adventure   = adventureContent,
            StartedAt   = DateTimeOffset.Now,
            LastSavedAt = DateTimeOffset.Now
        }
    }
}
