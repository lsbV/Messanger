namespace Core;

public abstract record Chat(ChatId Id);
public record ChatId(Guid Value);

public record PrivateChat(ChatId Id, UserId UserId1, UserId UserId2) : Chat(Id)
{
    private readonly List<Message> _messages = [];
    public IReadOnlyList<Message> Messages => _messages;
    public User? User1 { get; init; }
    public User? User2 { get; init; }
    public static PrivateChat Create(ChatId id, UserId userId1, UserId userId2)
    {
        return new PrivateChat(id, userId1, userId2);
    }
}


public record GroupChat(ChatId Id, ChatName ChatName) : Chat(Id)
{
    private readonly List<User> _users = [];
    public IReadOnlyList<User> Users => _users;
    private readonly List<Message> _messages = [];
    public IReadOnlyList<Message> Messages => _messages;
    public static GroupChat Create(ChatId id, ChatName chatName)
    {
        return new GroupChat(id, chatName);
    }
}


public record ChatName(string Value)
{
    public static ChatName Of(string value)
    {
        return new ChatName(value);
    }
}

public record Role(string Value)
{
    public static Role Of(string value)
    {
        return new Role(value);
    }
}