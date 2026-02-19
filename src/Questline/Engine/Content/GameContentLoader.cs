using Questline.Domain.Rooms.Data;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Framework.FileSystem;

using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Engine.Content;

public class GameContentLoader(JsonFileLoader loader)
{
    public WorldContent Load()
    {
        var adventureData = loader.LoadFile<AdventureData>("the-goblins-lair.json");

        var itemsDictionary = adventureData.Items.ToDictionary(i => i.Id, i => new Item
        {
            Id = i.Id,
            Description = i.Description,
            Name = i.Name
        });

        var barriers = adventureData.Barriers.ToDictionary(b => b.Id, b => new Barrier
        {
            Id = b.Id,
            Name = b.Name,
            Description = b.Description,
            BlockedMessage = b.BlockedMessage,
            UnlockItemId = b.UnlockItemId,
            UnlockMessage = b.UnlockMessage
        });

        var rooms = BuildRooms(adventureData.Rooms, itemsDictionary);

        return new WorldContent(rooms, barriers, adventureData.StartingRoomId);
    }

    private static Dictionary<string, Room> BuildRooms(
        RoomData[] roomData, Dictionary<string, Item> items)
    {
        var rooms = new Dictionary<string, Room>();

        foreach (var roomDetail in roomData)
        {
            var exits = new Dictionary<Direction, Exit>();
            if (roomDetail.Exits.Count > 0)
            {
                foreach (var (directionStr, exitDto) in roomDetail.Exits)
                {
                    var direction = Enum.Parse<Direction>(directionStr, true);
                    exits[direction] = new Exit(exitDto.Destination, exitDto.Barrier);
                }
            }

            var features = roomDetail.Features.Select(f => new Feature
            {
                Id = f.Id,
                Name = f.Name,
                Keywords = f.Keywords,
                Description = f.Description
            }).ToList();

            var room = new Room
            {
                Id = roomDetail.Id,
                Name = roomDetail.Name,
                Description = roomDetail.Description,
                Exits = exits,
                Features = features
            };

            if (roomDetail.Items.Length > 0)
            {
                foreach (var item in roomDetail.Items)
                {
                    room.Items.Add(items[item]);
                }
            }

            rooms[roomDetail.Id] = room;
        }

        return rooms;
    }
}
