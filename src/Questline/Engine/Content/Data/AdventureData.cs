namespace Questline.Engine.Content.Data;

public class AdventureData
{
    public string        Id             { get; set; } = null!;
    public string        Name           { get; set; } = null!;
    public string        StartingRoomId { get; set; } = null!;
    public RoomData[]    Rooms          { get; set; } = [];
    public ItemData[]    Items          { get; set; } = [];
    public BarrierData[] Barriers       { get; set; } = [];
}
