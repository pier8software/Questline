using Questline.Domain.Rooms.Entity;
using Questline.Engine.Repositories;
using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Rooms;

public class RoomRepository(
    IDataContext                        dataContext,
    IPersistenceMapper<Room, RoomDocument> mapper)
    : Repository<Room, RoomDocument>(dataContext, mapper), IRoomRepository
{
    public async Task<Room> GetById(string roomId) => await Load(roomId);
}
