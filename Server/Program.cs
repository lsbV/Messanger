using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;
using Core;
using Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
    options.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole()));
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new ContentDtoJsonConverter());
});
var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", async (AppDbContext context) =>
{
    var res = await context.Messages
        .Where(m => EF.Functions.Like((string)m.Content, "Text:%"))
        .Include(m => m.Sender)
        .Select(m => m.ToDto())
        .FirstOrDefaultAsync();

    return res;
});


await app.RunAsync();

internal static class MappingExtensions
{
    public static MessageDto ToDto(this Message message)
    {
        var contentAndType = message.Content switch
        {
            TextContent text => (text.Value, nameof(TextContent)),
            //ImageContent image => (image.Url, nameof(ImageContent)),
            //VideoContent video => (video.Url, nameof(VideoContent)),
            //AudioContent audio => (audio.Url, nameof(AudioContent)),
            //FileContent file => (file.Url, nameof(FileContent)),
            _ => throw new ArgumentOutOfRangeException(nameof(message.Content))

        };
        var dto = new MessageDto(
            message.Id.Value.ToString(),
            message.SenderId.Value.ToString(),
            message.Sender.Name.Value,
            message.RecipientId.Value.ToString(),
            message.Content.ToDto(),
            message.Status.ToString(),
            message.CreatedAt.ToString(CultureInfo.CurrentCulture),
            message.EditedAt?.ToString()
            );

        return dto;

    }

    public static ContentDto ToDto(this MessageContent content)
    {
        return content switch
        {
            TextContent text => new TextContentDto("Text", text.Value),
            //ImageContent image => new ImageContentDto(nameof(ImageContent), image.Url),
            //VideoContent video => new VideoContentDto(nameof(VideoContent), video.Url),
            //AudioContent audio => new AudioContentDto(nameof(AudioContent), audio.Url),
            //FileContent file => new FileContentDto(nameof(FileContent), file.Url),
            _ => throw new ArgumentOutOfRangeException(nameof(content))
        };
    }

}

internal record MessageDto(
    string Id,
    string SenderId,
    string Sender,
    string RecipientId,
    ContentDto Content,
    string Status,
    string CreatedAt,
    string? EditedAt
    );

internal abstract record ContentDto(
    string Type);

internal record TextContentDto(
    string Type,
    string Text) : ContentDto(Type);

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