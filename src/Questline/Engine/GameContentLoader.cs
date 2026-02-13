using Questline.Domain;
using Questline.Domain.Rooms.Data;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Framework.FileSystem;

namespace Questline.Engine;

public class GameContentLoader(JsonFileLoader loader)
{
    public GameState Load()
    {
        var adventureData = loader.LoadFile<AdventureData>("the-goblins-lair.json");

        var itemsDictionary = adventureData.Items.ToDictionary(i => i.Id, i => new Item
        {
            Id = i.Id,
            Description = i.Description,
            Name = i.Name
        });
        var rooms = BuildRooms(adventureData.Rooms, itemsDictionary);
        var world = new World(rooms);

        return new GameState(world, new Player { Id = "Player1", Location = adventureData.StartingRoomId });
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
                    var direction = Enum.Parse<Direction>(directionStr, ignoreCase: true);
                    exits[direction] = new Exit(exitDto.Destination, exitDto.Barrier);
                }
            }

            var room = new Room
            {
                Id = roomDetail.Id,
                Name = roomDetail.Name,
                Description = roomDetail.Description,
                Exits = exits
            };

            if (roomDetail.Items.Count > 0)
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
