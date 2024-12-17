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
    private Message() : this(default!, default!, default!, default!, default!, default!, default!)
    {
    }

}

public record MessageId(Guid Value)
{
    public static MessageId Of(Guid value)
    {
        return new MessageId(value);
    }
}

public abstract record MessageContent
{
    public static explicit operator string(MessageContent v) => v.ToString();// This is a conversion operator for the Entity Framework Core
}

public record TextContent(string Value) : MessageContent
{
    public static TextContent Of(string value)
    {
        return new TextContent(value);
    }
}

public record ImageContent(string Url) : MessageContent
{
    public static ImageContent Of(string url)
    {
        return new ImageContent(url);
    }

}

public record VideoContent(string Url) : MessageContent
{
    public static VideoContent Of(string url)
    {
        return new VideoContent(url);
    }

}

public record AudioContent(string Url) : MessageContent
{
    public static AudioContent Of(string url)
    {
        return new AudioContent(url);
    }
}


public record FileContent(string Url) : MessageContent
{
    public static FileContent Of(string url)
    {
        return new FileContent(url);
    }
}


public enum MessageStatus
{
    Sent,
    Delivered,
    Read
}
