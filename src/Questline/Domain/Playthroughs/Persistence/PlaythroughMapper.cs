using Questline.Domain.Playthroughs.Entity;
using Questline.Framework.Persistence;

namespace Questline.Domain.Playthroughs.Persistence;

public class PlaythroughMapper : IPersistenceMapper<Playthrough, PlaythroughDocument>
{
    public Playthrough From(PlaythroughDocument document) => Playthrough.From(document);

    public PlaythroughDocument To(Playthrough entity) => new()
    {
        Id          = entity.Id,
        PlayerId    = entity.PlayerId,
        CharacterId = entity.CharacterId,
        StartedAt   = entity.StartedAt,
        UpdatedAt   = entity.LastSavedAt
    };
}
