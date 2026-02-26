using Questline.Domain.Rooms.Entity;
using Questline.Framework.Domain;
using Barrier = Questline.Domain.Rooms.Entity.Barrier;

namespace Questline.Domain.Adventures.Entity;

public class Adventure : DomainEntity
{
    private Adventure(string id, string name, string startingRoomId, Room[] rooms, Barrier[] barriers)
    {
        Id             = id;
        Name           = name;
        StartingRoomId = startingRoomId;
        Rooms          = rooms;
        Barriers       = barriers;
    }

    public string    Name           { get; init; }
    public string    StartingRoomId { get; init; }
    public Room[]    Rooms          { get; init; }
    public Barrier[] Barriers       { get; init; }


    public static Adventure Create(string id, string name, string startingRoomId, Room[] rooms, Barrier[] barriers) =>
        new(id, name, startingRoomId, rooms, barriers);
}
