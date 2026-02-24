using Questline.Domain.Players.Entity;
using Questline.Domain.Rooms.Entity;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Domain.Shared.Data;

public class GameState(
    Dictionary<string, Room> rooms,
    Player? player = null,
    Dictionary<string, Barrier>? barriers = null,
    string adventureId = "")
{
    private readonly Dictionary<string, Barrier> _barriers = barriers ?? new Dictionary<string, Barrier>();

    public string AdventureId { get; } = adventureId;

    public Player Player { get; private set; } = player!;

    public IReadOnlyDictionary<string, Barrier> Barriers => _barriers;

    public IReadOnlyCollection<Room> Rooms => rooms.Values;

    public void SetPlayer(Player player) => Player = player;


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
