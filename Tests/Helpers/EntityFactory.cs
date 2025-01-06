namespace Tests.Helpers;

internal static class EntityFactory
{
    public static User CreateAndAddToContextRandomUser(AppDbContext context)
    {
        var user = CreateRandomUser();
        context.Users.Add(user);
        return user;
    }
    public static UserId CreateRandomUserId() => new UserId(Guid.NewGuid());
    public static ChatId CreateRandomChatId() => new ChatId(Guid.NewGuid());

    public static User CreateRandomUser()
    {
        var uniqueId = Guid.NewGuid();
        var user = new User(
            new UserId(uniqueId),
            new UserName("User" + uniqueId),
            new Password(
                new PasswordHash("password_" + uniqueId),
                new PasswordSalt("salt_" + uniqueId)),
            new Email($"user{uniqueId}@email.com"),
            new Avatar("https://www.test.com/images/" + uniqueId),
            new AuthorizationVersion(1));
        return user;
    }

    public static PrivateChat CreateAndAddToContextRandomPrivateChat(AppDbContext context, UserId id1, UserId id2)
    {
        var chat = new PrivateChat(new ChatId(Guid.NewGuid()), id1, id2, DateTime.UtcNow);
        context.Chats.Add(chat);
        return chat;
    }

    public static PrivateChat CreateRandomPrivateChat() =>
        new PrivateChat(CreateRandomChatId(), CreateRandomUserId(), CreateRandomUserId(), DateTime.UtcNow);

    public static GroupChat CreateAndAddToContextRandomGroupChat(AppDbContext context, List<User> members)
    {
        var chat = CreateRandomGroupChat();
        context.Chats.Add(chat);
        var now = DateTime.UtcNow;
        if (members.Count != 0) // add the first user as owner
        {
            context.GroupChatUsers.Add(new GroupChatUser(chat.Id, members[0].Id, GroupChatRole.Owner, now));
        }
        if (members.Count > 1) // add the second user as manager
        {
            context.GroupChatUsers.Add(new GroupChatUser(chat.Id, members[1].Id, GroupChatRole.Manager, now));
        }
        for (var i = 2; i < members.Count; i++) // add the rest as users
        {
            context.GroupChatUsers.Add(new GroupChatUser(chat.Id, members[i].Id, GroupChatRole.User, now));
        }
        return chat;
    }
    public static GroupChat CreateRandomGroupChat()
    {
        var uniqueId = Guid.NewGuid();
        var chat = new GroupChat(
            new ChatId(uniqueId),
            new ChatName("GroupChat"),
            new ChatDescription("Group Chat Test Description " + uniqueId),
            new ChatImage("https://www.test.com/images/" + uniqueId),
            GroupChatJoinMode.Free,
            DateTime.UtcNow
        );
        return chat;
    }

    public static Message CreateRandomMessage(ChatId chatId, UserId senderId)
    {
        var uniqueId = Guid.NewGuid();

        return new Message(new MessageId(uniqueId), senderId, chatId, new TextContent("Hello " + uniqueId), MessageStatus.Sent, DateTime.UtcNow, null);
    }

    public static Message CreateRandomMessage() =>
        CreateRandomMessage(EntityFactory.CreateRandomChatId(), EntityFactory.CreateRandomUserId());

    public static MessageId RandomMessageId() => new MessageId(Guid.NewGuid());

    public static RawPassword GenerateRandomRawPassword()
    {
        var uniqueId = Guid.NewGuid();
        return new RawPassword("password_" + uniqueId);
    }
}