using System.Text.Json.Serialization;

namespace Questline.Domain.Data;

public class RoomData
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;

    [JsonConverter(typeof(ExitDictionaryConverter))]
    public Dictionary<string, ExitData> Exits { get; init; } = new();

    public string[] Items { get; init; } = [];
}
