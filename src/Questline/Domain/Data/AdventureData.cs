namespace Questline.Domain.Data;

public class AdventureData
{
    public string StartingRoomId { get; set; } = null!;
    public RoomData[] Rooms { get; set; } = [];
    public ItemData[] Items { get; set; } = [];
}
