using System.Text.Json;

namespace Questline.Framework.FileSystem;

public class JsonFileLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public T LoadFile<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Required content file not found: {filePath}", filePath);
        }

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json, JsonOptions)
               ?? throw new JsonException($"Failed to deserialize {filePath}: result was null.");
    }
}
