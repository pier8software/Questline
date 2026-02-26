using System.Text.Json;
using System.Text.Json.Serialization;
using Questline.Engine.Content.Data;

namespace Questline.Framework.Serialisation;

public class ExitDictionaryConverter : JsonConverter<Dictionary<string, ExitData>>
{
    public override Dictionary<string, ExitData> Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new Dictionary<string, ExitData>(StringComparer.OrdinalIgnoreCase);

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

            var exit = reader.TokenType switch
            {
                JsonTokenType.String => new ExitData { Destination = reader.GetString()! },
                JsonTokenType.StartObject => JsonSerializer.Deserialize<ExitData>(ref reader, options)!,
                _ => throw new JsonException(
                    $"Exit '{direction}' must be a string (room ID) or an object with 'destination'.")
            };

            result[direction] = exit;
        }

        throw new JsonException("Unexpected end of JSON while reading exits.");
    }

    public override void Write(
        Utf8JsonWriter writer, Dictionary<string, ExitData> value, JsonSerializerOptions options)
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
