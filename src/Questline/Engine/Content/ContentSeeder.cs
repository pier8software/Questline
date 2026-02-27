using System.Text.Json;
using Questline.Domain.Rooms.Entity;
using Questline.Engine.Persistence.Adventures;
using Questline.Engine.Persistence.Rooms;
using Questline.Framework.FileSystem;
using Questline.Framework.Persistence;

namespace Questline.Engine.Content;

public class ContentSeeder(IDataContext dataContext, JsonFileLoader fileLoader)
{
    public async Task SeedAdventure(string filePath, CancellationToken cancellationToken = default)
    {
        var content  = fileLoader.LoadFile<AdventureContentJson>(filePath);
        var items    = content.Items.ToDictionary(i => i.Id);
        var barriers = content.Barriers.ToDictionary(b => b.Id);

        var adventureDoc = new AdventureDocument
        {
            Id             = content.Id,
            Name           = content.Name,
            Description    = content.Description,
            StartingRoomId = content.StartingRoomId
        };
        await dataContext.StoreDocument(adventureDoc, cancellationToken);

        foreach (var room in content.Rooms)
        {
            var roomDoc = new RoomDocument
            {
                Id          = room.Id,
                Name        = room.Name,
                Description = room.Description,
                Exits       = ResolveExits(room.Exits, barriers),
                Items       = ResolveItems(room.Items, items),
                Features    = ResolveFeatures(room.Features)
            };
            await dataContext.StoreDocument(roomDoc, cancellationToken);
        }
    }

    private static Dictionary<string, ExitDocument> ResolveExits(
        Dictionary<string, JsonElement> exits,
        Dictionary<string, BarrierJson> barriers)
    {
        var result = new Dictionary<string, ExitDocument>();

        foreach (var (directionStr, value) in exits)
        {
            var direction = Enum.Parse<Direction>(directionStr, true).ToString();

            if (value.ValueKind == JsonValueKind.String)
            {
                result[direction] = new ExitDocument { Destination = value.GetString()! };
            }
            else if (value.ValueKind == JsonValueKind.Object)
            {
                var destination = value.GetProperty("destination").GetString()!;
                BarrierDocument? barrierDoc = null;

                if (value.TryGetProperty("barrier", out var barrierElement) &&
                    barrierElement.GetString() is { } barrierId &&
                    barriers.TryGetValue(barrierId, out var barrier))
                {
                    barrierDoc = new BarrierDocument
                    {
                        Id             = barrier.Id,
                        Name           = barrier.Name,
                        Description    = barrier.Description,
                        BlockedMessage = barrier.BlockedMessage,
                        UnlockItemId   = barrier.UnlockItemId,
                        UnlockMessage  = barrier.UnlockMessage
                    };
                }

                result[direction] = new ExitDocument
                {
                    Destination = destination,
                    Barrier     = barrierDoc
                };
            }
        }

        return result;
    }

    private static List<ItemDocument> ResolveItems(
        List<string>                itemIds,
        Dictionary<string, ItemJson> items)
    {
        return itemIds
            .Select(id => items[id])
            .Select(i => new ItemDocument
            {
                Id          = i.Id,
                Name        = i.Name,
                Description = i.Description
            })
            .ToList();
    }

    private static List<FeatureDocument> ResolveFeatures(List<FeatureJson> features)
    {
        return features
            .Select(f => new FeatureDocument
            {
                Id          = f.Id,
                Name        = f.Name,
                Keywords    = f.Keywords,
                Description = f.Description
            })
            .ToList();
    }
}
