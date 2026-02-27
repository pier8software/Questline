using Questline.Domain.Rooms.Entity;

namespace Questline.Engine.Repositories;

public interface IRoomRepository
{
    Task<Room> GetById(string adventureId, string roomId);
    Task<string> GetStartingRoomId(string adventureId);
}
