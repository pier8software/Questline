using Questline.Domain.Adventures.Entity;
using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Adventures;

public class AdventureMapper : IPersistenceMapper<Adventure, AdventureDocument>
{
    public Adventure From(AdventureDocument document) => new()
    {
        Id             = document.Id,
        Name           = document.Name,
        Description    = document.Description,
        StartingRoomId = document.StartingRoomId
    };

    public AdventureDocument To(Adventure entity) => new()
    {
        Id             = entity.Id,
        Name           = entity.Name,
        Description    = entity.Description,
        StartingRoomId = entity.StartingRoomId
    };
}
