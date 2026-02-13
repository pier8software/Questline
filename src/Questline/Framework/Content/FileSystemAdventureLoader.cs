using System.Text.Json;
using Questline.Domain;
using Questline.Framework.Content.Dtos;

namespace Questline.Framework.Content;

public class FileSystemAdventureLoader : IAdventureLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public Adventure Load(string adventurePath)
    {
        var manifest = LoadFile<AdventureManifestDto>(adventurePath, "adventure.json");
        var roomsFile = LoadFile<RoomsFileDto>(adventurePath, "rooms.json");
        var itemsFile = LoadFile<ItemsFileDto>(adventurePath, "items.json");

        var itemDtos = itemsFile.Items.ToDictionary(i => i.Id!, i => i);
        var roomDtos = roomsFile.Rooms.ToDictionary(r => r.Id!, r => r);

        ValidateStartingRoom(manifest.StartingRoomId!, roomDtos);
        ValidateExitReferences(roomDtos);
        ValidateItemReferences(roomDtos, itemDtos);
        ValidateReachability(manifest.StartingRoomId!, roomDtos);

        var items = BuildItems(itemDtos);
        var rooms = BuildRooms(roomDtos, items);
        var world = new World(rooms);

        return new Adventure
        {
            Id = manifest.Id!,
            Name = manifest.Name!,
            Description = manifest.Description!,
            World = world,
            StartingRoomId = manifest.StartingRoomId!
        };
    }

    private static T LoadFile<T>(string adventurePath, string fileName)
    {
        var filePath = Path.Combine(adventurePath, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Required content file not found: {fileName}", filePath);
        }

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json, JsonOptions)
               ?? throw new JsonException($"Failed to deserialize {fileName}: result was null.");
    }

    private static void ValidateStartingRoom(string startingRoomId, Dictionary<string, RoomDto> rooms)
    {
        if (!rooms.ContainsKey(startingRoomId))
        {
            throw new ContentValidationException(
                $"Starting room '{startingRoomId}' does not exist in rooms.json.");
        }
    }

    private static void ValidateExitReferences(Dictionary<string, RoomDto> rooms)
    {
        foreach (var room in rooms.Values)
        {
            if (room.Exits is null)
            {
                continue;
            }

            foreach (var (directionStr, exit) in room.Exits)
            {
                if (!Enum.TryParse<Direction>(directionStr, ignoreCase: true, out _))
                {
                    throw new ContentValidationException(
                        $"Room '{room.Id}' has invalid direction '{directionStr}'.");
                }

                if (!rooms.ContainsKey(exit.Destination))
                {
                    throw new ContentValidationException(
                        $"Room '{room.Id}' has exit to non-existent room '{exit.Destination}'.");
                }
            }
        }
    }

    private static void ValidateItemReferences(
        Dictionary<string, RoomDto> rooms, Dictionary<string, ItemDto> items)
    {
        foreach (var room in rooms.Values)
        {
            if (room.Items is null)
            {
                continue;
            }

            foreach (var itemId in room.Items)
            {
                if (!items.ContainsKey(itemId))
                {
                    throw new ContentValidationException(
                        $"Room '{room.Id}' references unknown item '{itemId}'.");
                }
            }
        }
    }

    private static void ValidateReachability(string startingRoomId, Dictionary<string, RoomDto> rooms)
    {
        var visited = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue(startingRoomId);
        visited.Add(startingRoomId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var room = rooms[currentId];

            if (room.Exits is null)
            {
                continue;
            }

            foreach (var exit in room.Exits.Values)
            {
                if (visited.Add(exit.Destination))
                {
                    queue.Enqueue(exit.Destination);
                }
            }
        }

        var orphans = rooms.Keys.Where(id => !visited.Contains(id)).ToList();
        if (orphans.Count > 0)
        {
            throw new ContentValidationException(
                $"Rooms unreachable from starting room: {string.Join(", ", orphans.Select(id => $"'{id}'"))}.");
        }
    }

    private static Dictionary<string, Item> BuildItems(Dictionary<string, ItemDto> itemDtos)
    {
        var items = new Dictionary<string, Item>();
        foreach (var dto in itemDtos.Values)
        {
            items[dto.Id!] = new Item
            {
                Id = dto.Id!,
                Name = dto.Name!,
                Description = dto.Description!
            };
        }

        return items;
    }

    private static Dictionary<string, Room> BuildRooms(
        Dictionary<string, RoomDto> roomDtos, Dictionary<string, Item> items)
    {
        var rooms = new Dictionary<string, Room>();

        foreach (var dto in roomDtos.Values)
        {
            var exits = new Dictionary<Direction, Exit>();
            if (dto.Exits is not null)
            {
                foreach (var (directionStr, exitDto) in dto.Exits)
                {
                    var direction = Enum.Parse<Direction>(directionStr, ignoreCase: true);
                    exits[direction] = new Exit(exitDto.Destination, exitDto.Barrier);
                }
            }

            var room = new Room
            {
                Id = dto.Id!,
                Name = dto.Name!,
                Description = dto.Description!,
                Exits = exits
            };

            if (dto.Items is not null)
            {
                foreach (var itemId in dto.Items)
                {
                    room.Items.Add(items[itemId]);
                }
            }

            rooms[dto.Id!] = room;
        }

        return rooms;
    }
}
