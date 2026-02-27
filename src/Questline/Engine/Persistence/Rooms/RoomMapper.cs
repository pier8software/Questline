using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Entity;
using Questline.Framework.Persistence;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Engine.Persistence.Rooms;

public class RoomMapper : IPersistenceMapper<Room, RoomDocument>
{
    public Room From(RoomDocument document)
    {
        var exits = new Dictionary<Direction, Exit>();
        foreach (var (directionStr, exitDoc) in document.Exits)
        {
            var direction = Enum.Parse<Direction>(directionStr, true);
            Barrier? barrier = exitDoc.Barrier is not null
                ? new Barrier
                {
                    Id             = exitDoc.Barrier.Id,
                    Name           = exitDoc.Barrier.Name,
                    Description    = exitDoc.Barrier.Description,
                    BlockedMessage = exitDoc.Barrier.BlockedMessage,
                    UnlockItemId   = exitDoc.Barrier.UnlockItemId,
                    UnlockMessage  = exitDoc.Barrier.UnlockMessage
                }
                : null;
            exits[direction] = new Exit(exitDoc.Destination, barrier);
        }

        var items = document.Items.Select(i => new Item
        {
            Id          = i.Id,
            Name        = i.Name,
            Description = i.Description
        }).ToList();

        var features = document.Features.Select(f => new Feature
        {
            Id          = f.Id,
            Name        = f.Name,
            Keywords    = f.Keywords,
            Description = f.Description
        }).ToList();

        return new Room
        {
            Id          = document.Id,
            Name        = document.Name,
            Description = document.Description,
            Exits       = exits,
            Items       = items,
            Features    = features
        };
    }

    public RoomDocument To(Room entity)
    {
        var exits = new Dictionary<string, ExitDocument>();
        foreach (var (direction, exit) in entity.Exits)
        {
            exits[direction.ToString()] = new ExitDocument
            {
                Destination = exit.Destination,
                Barrier = exit.Barrier is not null
                    ? new BarrierDocument
                    {
                        Id             = exit.Barrier.Id,
                        Name           = exit.Barrier.Name,
                        Description    = exit.Barrier.Description,
                        BlockedMessage = exit.Barrier.BlockedMessage,
                        UnlockItemId   = exit.Barrier.UnlockItemId,
                        UnlockMessage  = exit.Barrier.UnlockMessage
                    }
                    : null
            };
        }

        return new RoomDocument
        {
            Id          = entity.Id,
            Name        = entity.Name,
            Description = entity.Description,
            Exits       = exits,
            Items       = entity.Items.Select(i => new ItemDocument
            {
                Id          = i.Id,
                Name        = i.Name,
                Description = i.Description
            }).ToList(),
            Features = entity.Features.Select(f => new FeatureDocument
            {
                Id          = f.Id,
                Name        = f.Name,
                Keywords    = f.Keywords,
                Description = f.Description
            }).ToList()
        };
    }
}
