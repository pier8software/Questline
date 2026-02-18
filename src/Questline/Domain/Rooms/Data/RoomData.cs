using System.Text.Json.Serialization;
using Questline.Framework.Serialisation;

namespace Questline.Domain.Rooms.Data;

public class RoomData
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;

    [JsonConverter(typeof(ExitDictionaryConverter))]
    public Dictionary<string, ExitData> Exits { get; init; } = new();

    public string[] Items { get; init; } = [];
    public FeatureData[] Features { get; init; } = [];
}

public class ExitData
{
    public required string Destination { get; set; }
    public string? Barrier { get; set; }
}
