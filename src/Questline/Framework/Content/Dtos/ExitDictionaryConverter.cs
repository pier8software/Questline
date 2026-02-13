using System.Text.Json;
using System.Text.Json.Serialization;

namespace Questline.Framework.Content.Dtos;

public class ExitDictionaryConverter : JsonConverter<Dictionary<string, ExitDto>>
{
    public override Dictionary<string, ExitDto> Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new Dictionary<string, ExitDto>(StringComparer.OrdinalIgnoreCase);

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object for exits.");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return result;
            }

            var direction = reader.GetString()!;
            reader.Read();

            ExitDto exit = reader.TokenType switch
            {
                JsonTokenType.String => new ExitDto { Destination = reader.GetString()! },
                JsonTokenType.StartObject => JsonSerializer.Deserialize<ExitDto>(ref reader, options)!,
                _ => throw new JsonException(
                    $"Exit '{direction}' must be a string (room ID) or an object with 'destination'.")
            };

            result[direction] = exit;
        }

        throw new JsonException("Unexpected end of JSON while reading exits.");
    }

    public override void Write(
        Utf8JsonWriter writer, Dictionary<string, ExitDto> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var (direction, exit) in value)
        {
            writer.WritePropertyName(direction);
            if (exit.Barrier is null)
            {
                writer.WriteStringValue(exit.Destination);
            }
            else
            {
                JsonSerializer.Serialize(writer, exit, options);
            }
        }

        writer.WriteEndObject();
    }
}
