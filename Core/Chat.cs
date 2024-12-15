using System.Collections.Immutable;

namespace Core;

public record Chat(ChatId ChatId);

public record ChatId(Guid Value)
{
    public static ChatId Of(Guid value)
    {
        return new ChatId(value);
    }
}

public record PrivateChat(ChatId ChatId, UserId UserId1, UserId UserId2) : Chat(ChatId)
{
    public static PrivateChat Create(ChatId id, UserId userId1, UserId userId2)
    {
        return new PrivateChat(id, userId1, userId2);
    }
}

public record GroupChat(ChatId ChatId, ChatName ChatName, UserId Owner) : Chat(ChatId)
{
    private readonly List<User> _users = [];
    public IReadOnlyList<User> Users => _users;
    public static GroupChat Create(ChatId id, ChatName chatName, UserId owner)
    {
        return new GroupChat(id, chatName, owner);
    }
}

public record ChatName(string Value)
{
    public static ChatName Of(string value)
    {
        return new ChatName(value);
    }
}