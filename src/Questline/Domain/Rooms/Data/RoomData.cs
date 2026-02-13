using System.Text.Json.Serialization;
using Questline.Framework.Content.Dtos;

namespace Questline.Domain.Rooms.Data;

public class RoomData
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    [JsonConverter(typeof(ExitDictionaryConverter))]
    public Dictionary<string, ExitData> Exits { get; set; }

    public List<string> Items { get; set; } = [];
}
