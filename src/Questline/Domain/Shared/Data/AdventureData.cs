using Questline.Domain.Rooms.Data;

namespace Questline.Domain.Shared.Data;

public class AdventureData
{
    public string StartingRoomId { get; set; } = null!;
    public RoomData[] Rooms { get; set; } = [];
    public ItemData[] Items { get; set; } = [];
}
