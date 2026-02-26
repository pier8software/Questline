using Questline.Domain.Rooms.Entity;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Engine.Content;

public record AdventureContent(
    Dictionary<string, Room>    Rooms,
    Dictionary<string, Barrier> Barriers,
    string                      StartingRoomId)
{
    public Room GetRoom(string id)
    {
        if (!Rooms.TryGetValue(id, out var room))
        {
            throw new KeyNotFoundException($"Room '{id}' not found.");
        }

        return room;
    }

    public Barrier? GetBarrier(string? id)
    {
        if (id is null)
        {
            return null;
        }

        return Barriers.GetValueOrDefault(id);
    }
}
