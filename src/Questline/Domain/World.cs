namespace Questline.Domain;

public class World
{
    private readonly Dictionary<string, Room> rooms;

    public World(Dictionary<string, Room> rooms)
    {
        this.rooms = rooms;
    }

    public Room GetRoom(string id)
    {
        if (!rooms.TryGetValue(id, out var room))
        {
            throw new KeyNotFoundException($"Room '{id}' not found.");
        }

        return room;
    }
}
