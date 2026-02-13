using Questline.Domain.Entities;

namespace Questline.Domain.Shared;

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
