namespace Core;

public abstract record Chat(ChatId Id, DateTime CreatedAt)
{
    private readonly List<ChatEvent> _events = [];
    public IReadOnlyList<ChatEvent> Events => _events;
}
public record ChatId(Guid Value);

public abstract record ChatEvent(ChatEventId Id, ChatId ChatId, DateTime CreatedAt);

public record ChatEventId(Guid Value)
{
    public static ChatEventId New => new ChatEventId(Guid.NewGuid());
}

public record UserCreatedGroupChatEvent(ChatEventId Id, ChatId ChatId, UserId UserId, DateTime CreatedAt) : ChatEvent(Id, ChatId, CreatedAt);

public record UserCreatedPrivateChatEvent(ChatEventId Id, ChatId ChatId, UserId UserId, DateTime CreatedAt) : ChatEvent(Id, ChatId, CreatedAt);

public record UserJoinedGroupChatEvent(ChatEventId Id, ChatId ChatId, UserId UserId, DateTime CreatedAt) : ChatEvent(Id, ChatId, CreatedAt);

public record UserLeftGroupChatEvent(ChatEventId Id, ChatId ChatId, UserId UserId, DateTime CreatedAt) : ChatEvent(Id, ChatId, CreatedAt);

public record GroupChatImageUpdatedEvent(ChatEventId Id, ChatId ChatId, ChatImage ChatImage, DateTime CreatedAt) : ChatEvent(Id, ChatId, CreatedAt);

public record GroupChatNameUpdatedEvent(ChatEventId Id, ChatId ChatId, ChatName ChatName, DateTime CreatedAt) : ChatEvent(Id, ChatId, CreatedAt);

public record GroupChatDescriptionUpdatedEvent(ChatEventId Id, ChatId ChatId, ChatDescription ChatDescription, DateTime CreatedAt) : ChatEvent(Id, ChatId, CreatedAt);

public record ChatDeletedEvent(ChatEventId Id, ChatId ChatId, DateTime CreatedAt) : ChatEvent(Id, ChatId, CreatedAt);



public record PrivateChat(ChatId Id, UserId UserId1, UserId UserId2, DateTime CreatedAt) : Chat(Id, CreatedAt)
{
    public User? User1 { get; init; }
    public User? User2 { get; init; }
    public static PrivateChat Create(ChatId id, UserId userId1, UserId userId2)
    {
        return new PrivateChat(id, userId1, userId2, DateTime.UtcNow);
    }
}


public record GroupChat(ChatId Id, ChatName ChatName, ChatDescription ChatDescription, ChatImage ChatImage, GroupChatJoinMode JoinMode, DateTime CreatedAt) : Chat(Id, CreatedAt)
{
    private readonly List<User> _users = [];
    public IReadOnlyList<User> Users => _users;
    public static GroupChat Create(ChatName chatName, ChatDescription chatDescription, ChatImage chatImage, GroupChatJoinMode joinMode)
    {
        return new GroupChat(new ChatId(Guid.NewGuid()), chatName, chatDescription, chatImage, joinMode, DateTime.UtcNow);
    }
}

public record ChatImage(string Url);
public record ChatDescription(string Value);
public record ChatName(string Value);

public enum GroupChatRole
{
    Owner,
    Manager,
    User,

}

public enum GroupChatJoinMode
{
    Free,
    Invitation
}