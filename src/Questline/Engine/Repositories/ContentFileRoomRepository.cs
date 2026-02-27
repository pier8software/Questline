using Questline.Domain.Rooms.Data;
using Questline.Domain.Rooms.Entity;
using Questline.Domain.Shared.Data;
using Questline.Domain.Shared.Entity;
using Questline.Framework.FileSystem;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Engine.Repositories;

public class ContentFileRoomRepository(JsonFileLoader loader) : IRoomRepository
{
    private readonly Dictionary<string, AdventureCache> _cache = new();

    public Task<Room> GetById(string adventureId, string roomId)
    {
        var cache = GetOrLoadAdventure(adventureId);

        if (!cache.Rooms.TryGetValue(roomId, out var room))
        {
            throw new KeyNotFoundException($"Room '{roomId}' not found in adventure '{adventureId}'.");
        }

        return Task.FromResult(room);
    }

    public Task<string> GetStartingRoomId(string adventureId)
    {
        var cache = GetOrLoadAdventure(adventureId);
        return Task.FromResult(cache.StartingRoomId);
    }

    private AdventureCache GetOrLoadAdventure(string adventureId)
    {
        if (_cache.TryGetValue(adventureId, out var cache))
        {
            return cache;
        }

        cache = LoadAdventure(adventureId);
        _cache[adventureId] = cache;
        return cache;
    }

    private AdventureCache LoadAdventure(string adventureId)
    {
        var adventureData = loader.LoadFile<AdventureData>($"{adventureId}.json");

        var items = adventureData.Items.ToDictionary(i => i.Id, i => new Item
        {
            Id          = i.Id,
            Name        = i.Name,
            Description = i.Description
        });

        var barriers = adventureData.Barriers.ToDictionary(b => b.Id, b => new Barrier
        {
            Id             = b.Id,
            Name           = b.Name,
            Description    = b.Description,
            BlockedMessage = b.BlockedMessage,
            UnlockItemId   = b.UnlockItemId,
            UnlockMessage  = b.UnlockMessage
        });

        var rooms = BuildRooms(adventureData.Rooms, items, barriers);

        return new AdventureCache(rooms, adventureData.StartingRoomId);
    }

    private static Dictionary<string, Room> BuildRooms(
        RoomData[]                  roomData,
        Dictionary<string, Item>    items,
        Dictionary<string, Barrier> barriers)
    {
        var rooms = new Dictionary<string, Room>();

        foreach (var roomDetail in roomData)
        {
            var exits = new Dictionary<Direction, Exit>();
            foreach (var (directionStr, exitDto) in roomDetail.Exits)
            {
                var direction = Enum.Parse<Direction>(directionStr, true);
                var barrier   = exitDto.Barrier is not null ? barriers.GetValueOrDefault(exitDto.Barrier) : null;
                exits[direction] = new Exit(exitDto.Destination, barrier);
            }

            var features = roomDetail.Features.Select(f => new Feature
            {
                Id          = f.Id,
                Name        = f.Name,
                Keywords    = f.Keywords,
                Description = f.Description
            }).ToList();

            var roomItems = roomDetail.Items
                .Select(itemId => items[itemId]).ToList();

            rooms[roomDetail.Id] = new Room
            {
                Id          = roomDetail.Id,
                Name        = roomDetail.Name,
                Description = roomDetail.Description,
                Exits       = exits,
                Items       = roomItems,
                Features    = features
            };
        }

        return rooms;
    }

    private record AdventureCache(Dictionary<string, Room> Rooms, string StartingRoomId);
}
