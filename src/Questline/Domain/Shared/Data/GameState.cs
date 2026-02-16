using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;

namespace Questline.Domain.Shared.Data;

public class GameState(Dictionary<string, Room> rooms, Player player)
{
    public Player Player { get; } = player;

    public Room GetRoom(string id)
    {
        if (!rooms.TryGetValue(id, out var room))
        {
            throw new KeyNotFoundException($"Room '{id}' not found.");
        }

        return room;
    }
}
