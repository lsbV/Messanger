using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.Converters;

internal class ContentDtoJsonConverter : JsonConverter<MessageContentDto>
{
    public override MessageContentDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var type = doc.RootElement.GetProperty(nameof(MessageContentDto.Type)).GetString();

        return type switch
        {
            "Text" => JsonSerializer.Deserialize<TextMessageContentDto>(doc.RootElement.GetRawText(), options),
            _ => throw new JsonException($"Unknown type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, MessageContentDto value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}