using Questline.Domain.Rooms.Persistence;
using Questline.Framework.Persistence;

namespace Questline.Domain.Adventures.Persistence;

public class AdventureDocument : Document
{
    public string            Name           { get; set; }
    public string            StartingRoomId { get; set; }
    public RoomDocument[]    Rooms          { get; set; }
    public BarrierDocument[] Barriers       { get; set; }
}
