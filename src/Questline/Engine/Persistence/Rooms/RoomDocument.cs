using Questline.Framework.Persistence;

namespace Questline.Engine.Persistence.Rooms;

public class RoomDocument : Document
{
    public string                             Name        { get; set; } = null!;
    public string                             Description { get; set; } = null!;
    public Dictionary<string, ExitDocument>   Exits       { get; set; } = new();
    public List<ItemDocument>                 Items       { get; set; } = [];
    public List<FeatureDocument>              Features    { get; set; } = [];
}

public class ExitDocument
{
    public string           Destination { get; set; } = null!;
    public BarrierDocument? Barrier     { get; set; }
}

public class BarrierDocument
{
    public string Id             { get; set; } = null!;
    public string Name           { get; set; } = null!;
    public string Description    { get; set; } = null!;
    public string BlockedMessage { get; set; } = null!;
    public string UnlockItemId   { get; set; } = null!;
    public string UnlockMessage  { get; set; } = null!;
}

public class ItemDocument
{
    public string Id          { get; set; } = null!;
    public string Name        { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class FeatureDocument
{
    public string   Id          { get; set; } = null!;
    public string   Name        { get; set; } = null!;
    public string[] Keywords    { get; set; } = [];
    public string   Description { get; set; } = null!;
}
