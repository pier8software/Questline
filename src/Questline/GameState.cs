namespace Questline;

public record RoomState
{
    public bool Visited { get; init; }
    public bool IsLit { get; init; } = true;
}

public record GameState
{
    public string CurrentRoomId { get; init; } = "start";
    public IReadOnlyList<string> Inventory { get; init; } = [];
    public IReadOnlyDictionary<string, RoomState> Rooms { get; init; } = new Dictionary<string, RoomState>();
    public IReadOnlyDictionary<string, bool> Flags { get; init; } = new Dictionary<string, bool>();

    public GameState WithFlag(string flag, bool value)
    {
        var newFlags = new Dictionary<string, bool>(Flags) { [flag] = value };
        return this with { Flags = newFlags };
    }

    public GameState WithRoom(string roomId)
    {
        return this with { CurrentRoomId = roomId };
    }

    public GameState WithRoomVisited(string roomId)
    {
        var newRooms = new Dictionary<string, RoomState>(Rooms);
        var currentState = Rooms.GetValueOrDefault(roomId, new RoomState());
        newRooms[roomId] = currentState with { Visited = true };
        return this with { Rooms = newRooms };
    }
}
