namespace Core;

public record Message(
    MessageId Id,
    SenderId SenderId,
    RecipientId RecipientId,
    MessageContent Content,
    MessageStatus Status,
    DateTime CreatedAt,
    DateTime? EditedAt = null
    );

public record MessageId(Guid Value)
{
    public static Guid Of(Guid value)
    {
        return value;
    }
}


public record SenderId(Guid Value)
{
    public static Guid Of(Guid value)
    {
        return value;
    }
}

public record RecipientId(Guid Value)
{
    public static Guid Of(Guid value)
    {
        return value;
    }
}

public abstract record MessageContent(TextMessageContent MainContent, AdditionContent? AdditionContent);

public abstract record AdditionContent;
public abstract record MainContent;
public record TextMessageContent(string Value) : MainContent;

public record ImageMessageContent(string Url) : AdditionContent;

public record VideoMessageContent(string Url) : AdditionContent;

public record AudioMessageContent(string Url) : AdditionContent;

public record FileMessageContent(string Url) : AdditionContent;

public enum MessageStatus
{
    Sent,
    Delivered,
    Read
}
