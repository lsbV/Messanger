using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.Converters;

internal class ContentDtoJsonConverter : JsonConverter<ContentDto>
{
    public override ContentDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var type = doc.RootElement.GetProperty("Type").GetString();

        return type switch
        {
            "Text" => JsonSerializer.Deserialize<TextContentDto>(doc.RootElement.GetRawText(), options),
            _ => throw new JsonException($"Unknown type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, ContentDto value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}