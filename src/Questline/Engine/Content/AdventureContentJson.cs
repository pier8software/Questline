using System.Text.Json;

namespace Questline.Engine.Content;

public class AdventureContentJson
{
    public string                          Id             { get; set; } = null!;
    public string                          Name           { get; set; } = null!;
    public string                          Description    { get; set; } = null!;
    public string                          StartingRoomId { get; set; } = null!;
    public List<RoomJson>                  Rooms          { get; set; } = [];
    public List<ItemJson>                  Items          { get; set; } = [];
    public List<BarrierJson>               Barriers       { get; set; } = [];
}

public class RoomJson
{
    public string                          Id          { get; set; } = null!;
    public string                          Name        { get; set; } = null!;
    public string                          Description { get; set; } = null!;
    public Dictionary<string, JsonElement> Exits       { get; set; } = new();
    public List<string>                    Items       { get; set; } = [];
    public List<FeatureJson>               Features    { get; set; } = [];
}

public class ItemJson
{
    public string Id          { get; set; } = null!;
    public string Name        { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class BarrierJson
{
    public string Id             { get; set; } = null!;
    public string Name           { get; set; } = null!;
    public string Description    { get; set; } = null!;
    public string BlockedMessage { get; set; } = null!;
    public string UnlockItemId   { get; set; } = null!;
    public string UnlockMessage  { get; set; } = null!;
}

public class FeatureJson
{
    public string   Id          { get; set; } = null!;
    public string   Name        { get; set; } = null!;
    public string[] Keywords    { get; set; } = [];
    public string   Description { get; set; } = null!;
}
