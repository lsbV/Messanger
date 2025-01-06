namespace Core;

public record Message(
    MessageId Id,
    UserId SenderId,
    ChatId RecipientId,
    MessageContent Content,
    MessageStatus Status,
    DateTime CreatedAt,
    DateTime? EditedAt
)
{
    //private Message() : this(default!, default!, default!, default!, default!, default!, default!)
    //{
    //}
}

public record MessageId(Guid Value);

public abstract record MessageContent;

public record DeletedContent(DateTime DeletedAt) : MessageContent;

public record TextContent(string Value) : MessageContent;

public abstract record MediaContent(
    string Url,
    string Name,
    uint Size,
    IReadOnlyDictionary<string, string> Metadata) : MessageContent;

public record ImageContent(string Url, string Name, uint Size)
    : MediaContent(Url, Name, Size, new Dictionary<string, string>());

public record VideoContent(string Url, string Name, uint Size, uint Duration)
    : MediaContent(Url, Name, Size, new Dictionary<string, string>());

public record AudioContent(string Url, string Name, uint Size, uint Duration)
    : MediaContent(Url, Name, Size, new Dictionary<string, string>());

public record FileContent(string Url, string Name, uint Size)
    : MediaContent(Url, Name, Size, new Dictionary<string, string>());

public enum MessageStatus
{
    Sent,
    Delivered,
    Read
}
