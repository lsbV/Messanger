namespace Core;

public record Message(
    MessageId Id,
    UserId SenderId,
    User Sender,
    ChatId RecipientId,
    Chat Recipient,
    MessageStatus Status,
    DateTime CreatedAt,
    DateTime? EditedAt
)
{
    public static Message Create(
        MessageId id,
        UserId senderId,
        User sender,
        ChatId recipientId,
        Chat recipient,
        MessageStatus status,
        DateTime createdAt,
        DateTime? editedAt)
    {
        return new Message(id, senderId, sender, recipientId, recipient, status, createdAt, editedAt);
    }

    private Message() : this(default!, default!, default!, default!, default!, default!, default!, default!)
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

public record TextMessage(
    MessageId Id,
    UserId SenderId,
    User Sender,
    ChatId RecipientId,
    Chat Recipient,
    MessageStatus Status,
    DateTime CreatedAt,
    DateTime? EditedAt,
    TextContent Text)
    : Message(Id, SenderId, Sender, RecipientId, Recipient, Status, CreatedAt, EditedAt)
{
    private TextMessage() : this(default!, default!, default!, default!, default!, default!, default!, default!, default!)
    {
    }
}

public record TextContent(string Value)
{
    public static TextContent Of(string value)
    {
        return new TextContent(value);
    }

}

public record ImageMessage(
    MessageId Id,
    UserId SenderId,
    User Sender,
    ChatId RecipientId,
    Chat Recipient,
    MessageStatus Status,
    DateTime CreatedAt,
    DateTime? EditedAt,
    ImageContent Image)
    : Message(Id, SenderId, Sender, RecipientId, Recipient, Status, CreatedAt, EditedAt)
{
    private ImageMessage() : this(default!, default!, default!, default!, default!, default!, default!, default!, default!)
    {
    }
}

public record ImageContent(string Url)
{
    public static ImageContent Of(string url)
    {
        return new ImageContent(url);
    }

}

public record VideoMessage(
    MessageId Id,
    UserId SenderId,
    User Sender,
    ChatId RecipientId,
    Chat Recipient,
    MessageStatus Status,
    DateTime CreatedAt,
    DateTime? EditedAt,
    VideoContent Video)
    : Message(Id, SenderId, Sender, RecipientId, Recipient, Status, CreatedAt, EditedAt)
{
    private VideoMessage() : this(default!, default!, default!, default!, default!, default!, default!, default!, default!)
    {
    }
}

public record VideoContent(string Url)
{
    public static VideoContent Of(string url)
    {
        return new VideoContent(url);
    }

}

public record AudioMessage(
    MessageId Id,
    UserId SenderId,
    User Sender,
    ChatId RecipientId,
    Chat Recipient,
    MessageStatus Status,
    DateTime CreatedAt,
    DateTime? EditedAt,
    AudioContent File)
    : Message(Id, SenderId, Sender, RecipientId, Recipient, Status, CreatedAt, EditedAt)
{
    private AudioMessage() : this(default!, default!, default!, default!, default!, default!, default!, default!, default!)
    {
    }
}

public record AudioContent(string Url)
{
    public static AudioContent Of(string url)
    {
        return new AudioContent(url);
    }
}

public record FileMessage(
    MessageId Id,
    UserId SenderId,
    User Sender,
    ChatId RecipientId,
    Chat Recipient,
    MessageStatus Status,
    DateTime CreatedAt,
    DateTime? EditedAt,
    FileContent File)
    : Message(Id, SenderId, Sender, RecipientId, Recipient, Status, CreatedAt, EditedAt)
{
    private FileMessage() : this(default!, default!, default!, default!, default!, default!, default!, default!, default!)
    {
    }
}

public record FileContent(string Url)
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
