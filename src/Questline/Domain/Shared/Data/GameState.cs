using Questline.Domain.Rooms.Entity;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Domain.Shared.Data;

public class GameState(Dictionary<string, Room> rooms, Dictionary<string, Barrier>? barriers = null)
{
    private readonly Dictionary<string, Barrier> _barriers = barriers ?? new Dictionary<string, Barrier>();

    public IReadOnlyDictionary<string, Barrier> Barriers => _barriers;

    public Room GetRoom(string id)
    {
        if (!rooms.TryGetValue(id, out var room))
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

        return _barriers.GetValueOrDefault(id);
    }
}
