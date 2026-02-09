namespace Questline.Domain;

public class World(Dictionary<string, Room> rooms)
{
    public Room GetRoom(string id)
    {
        if (!rooms.TryGetValue(id, out var room))
        {
            throw new KeyNotFoundException($"Room '{id}' not found.");
        }

        return room;
    }
}
