using System.Globalization;
using Core;

internal static class MappingExtensions
{
    public static MessageDto ToDto(this Message message)
    {
        var contentAndType = message.Content switch
        {
            TextContent text => (text.Value, nameof(TextContent)),
            _ => throw new ArgumentOutOfRangeException(nameof(message.Content))

        };
        var dto = new MessageDto(
            message.Id.Value.ToString(),
            message.SenderId.Value.ToString(),
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
            _ => throw new ArgumentOutOfRangeException(nameof(content))
        };
    }

}