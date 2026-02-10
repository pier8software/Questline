using System.Text.Json.Serialization;

namespace Questline.Framework.Content.Dtos;

public class RoomDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    [JsonConverter(typeof(ExitDictionaryConverter))]
    public Dictionary<string, ExitDto>? Exits { get; set; }

    public List<string>? Items { get; set; }
}
