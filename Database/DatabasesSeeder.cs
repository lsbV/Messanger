using Azure.Storage.Blobs;
using Database.Models;
using MongoDB.Driver;

namespace Database;

public class DatabasesSeeder
{
    public static async Task Seed(AppDbContext context, IMongoCollection<Message> messageCollection, CancellationToken cancellationToken)
    {
        var users = Enumerable.Range(0, 10).Select(_ => CreateUser()).ToList();
        context.Users.AddRange(users);

        var privateChats = users.SelectMany(
            user1 => users.Where(user2 => user1.Id != user2.Id).Select(user2 => CreatePrivateChat(user1, user2))).ToList();

        var groupChats = users.Take(users.Count / 2).Select(CreateGroupChat).ToList();

        context.GroupChats.AddRange(groupChats);
        context.PrivateChats.AddRange(privateChats);

        var groupChatUsers = groupChats.SelectMany(
            groupChat => users.Skip(users.Count / 2 + 1)
                .Select(user => new GroupChatUser(groupChat.Id, user.Id, GroupChatRole.User, groupChat.CreatedAt.AddHours(4)))).ToList();

        context.GroupChatUsers.AddRange(groupChatUsers);

        var messagesToPrivateChat = new List<Message>();

        foreach (var chat in privateChats)
        {
            var messages = CreateMessages(chat, [chat.UserId1, chat.UserId2]);
            messagesToPrivateChat.AddRange(messages);
        }

        var messagesToGroupChat = new List<Message>();

        foreach (var chat in groupChats)
        {
            var messages = CreateMessages(chat, groupChatUsers.Where(u => u.GroupChatId == chat.Id).Select(groupChatUser => groupChatUser.UserId).ToList());
            messagesToGroupChat.AddRange(messages);
        }
        await context.SaveChangesAsync(cancellationToken);

        await messageCollection.InsertManyAsync(messagesToPrivateChat.Concat(messagesToGroupChat), null, cancellationToken);




    }

    private static User CreateUser()
    {
        List<string> randomNames =
        [
            "John", "Jane", "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Heidi", "Ivan", "Jack",
            "Kate", "Liam", "Mia", "Nina", "Oliver", "Pam", "Quinn", "Riley", "Sara", "Tom", "Uma", "Violet",
            "Will", "Xander", "Yara", "Zoe"
        ];
        var random = new Random();
        var id = UserId.New();
        var name = randomNames[random.Next(randomNames.Count)];
        var email = $"{name + id.Value.ToString().Replace("-", "")}@email.com";
        var password = new Password(new PasswordHash("bde8efa7a53306b1df1e6d9eb9e87d69405b80e7b1faf0b079add7658674c651"),
            new PasswordSalt("0cf7f667ae95357007785f3bd8b6ac5f8512e822cb43a453617767edccef37be")); // password
        var avatar = new Avatar("/avatar/" + id.Value);
        var authorizationVersion = new AuthorizationVersion(1);
        return new User(id, new UserName(name), password, new Email(email), avatar, authorizationVersion);
    }

    private static PrivateChat CreatePrivateChat(User user1, User user2)
    {
        var id = new ChatId(Guid.NewGuid());
        var random = new Random();
        var date = DateTime.UtcNow.AddDays(-random.Next(7));
        return new PrivateChat(id, user1.Id, user2.Id, date);
    }

    private static GroupChat CreateGroupChat(User user)
    {
        var id = new ChatId(Guid.NewGuid());
        var random = new Random();
        var date = DateTime.UtcNow.AddDays(-random.Next(7));
        var chatName = new ChatName("Chat" + id.Value);
        var chatDescription = new ChatDescription("Description" + id.Value);
        var chatImage = new ChatImage("/chat/" + id.Value);
        return new GroupChat(id, chatName, chatDescription, chatImage, GroupChatJoinMode.Free, date);
    }

    private static ChatEvent CreateUserCreatedGroupChatEvent(User user, GroupChat groupChat)
    {
        var id = ChatEventId.New;
        return new UserCreatedGroupChatEvent(id, groupChat.Id, user.Id, groupChat.CreatedAt);
    }

    private static ChatEvent CreateUserCreatedPrivateChatEvent(User user, PrivateChat privateChat)
    {
        var id = ChatEventId.New;
        return new UserCreatedPrivateChatEvent(id, privateChat.Id, user.Id, privateChat.CreatedAt);
    }

    private static ChatEvent CreateUserJoinedGroupChatEvent(User user, GroupChat groupChat)
    {
        var id = ChatEventId.New;
        return new UserJoinedGroupChatEvent(id, groupChat.Id, user.Id, groupChat.CreatedAt);
    }

    private static ChatEvent CreateUserLeftGroupChatEvent(User user, GroupChat groupChat)
    {
        var id = ChatEventId.New;
        return new UserLeftGroupChatEvent(id, groupChat.Id, user.Id, groupChat.CreatedAt);
    }

    private static ChatEvent CreateGroupChatImageUpdatedEvent(User user, GroupChat groupChat)
    {
        var id = ChatEventId.New;
        var random = new Random();
        var eventDate = groupChat.CreatedAt.AddDays(random.Next(7));
        return new GroupChatImageUpdatedEvent(id, groupChat.Id, groupChat.ChatImage, eventDate);
    }

    private static ChatEvent CreateGroupChatNameUpdatedEvent(User user, GroupChat groupChat)
    {
        var id = ChatEventId.New;
        var random = new Random();
        var eventDate = groupChat.CreatedAt.AddDays(random.Next(7));
        return new GroupChatNameUpdatedEvent(id, groupChat.Id, groupChat.ChatName, eventDate);
    }

    private static ChatEvent CreateGroupChatDescriptionUpdatedEvent(User user, GroupChat groupChat)
    {
        var id = ChatEventId.New;
        var random = new Random();
        var eventDate = groupChat.CreatedAt.AddDays(random.Next(-7, 0));
        return new GroupChatDescriptionUpdatedEvent(id, groupChat.Id, groupChat.ChatDescription, eventDate);
    }

    private static ChatEvent CreateChatDeletedEvent(User user, Chat chat)
    {
        var id = ChatEventId.New;
        return new ChatDeletedEvent(id, chat.Id, DateTime.UtcNow);
    }

    private static ChatEvent CreateChatEvent(User user, Chat chat)
    {
        var random = new Random();
        var eventType = random.Next(6);
        return eventType switch
        {
            0 => CreateUserCreatedGroupChatEvent(user, (GroupChat)chat),
            1 => CreateUserCreatedPrivateChatEvent(user, (PrivateChat)chat),
            2 => CreateUserJoinedGroupChatEvent(user, (GroupChat)chat),
            3 => CreateUserLeftGroupChatEvent(user, (GroupChat)chat),
            4 => CreateGroupChatImageUpdatedEvent(user, (GroupChat)chat),
            5 => CreateGroupChatNameUpdatedEvent(user, (GroupChat)chat),
            6 => CreateGroupChatDescriptionUpdatedEvent(user, (GroupChat)chat),
            _ => CreateChatDeletedEvent(user, chat)
        };
    }

    private static Message CreateMessage(UserId userId, Chat chat, MessageStatus status, DateTime date)
    {
        var id = new MessageId(Guid.NewGuid());
        var random = new Random();
        //MessageContent content = random.Next(3) switch
        //{
        //    0 => new TextContent("Text message"),
        //    1 => new ImageContent("/attacmente/" + id, "Image", 100),
        //    2 => new VideoContent("/attacmente/" + id, "Video", 100, 10),
        //    3 => new AudioContent("/attacmente/" + id, "Audio", 100, 10),
        //    _ => new FileContent("/attacmente/" + id, "File", 100)
        //};
        var content = new TextContent("Text message" + random.Next(1000));
        return new Message(id, userId, chat.Id, content, status, date, null);
    }

    private static List<Message> CreateMessages(Chat chat, List<UserId> userIds)
    {
        var random = new Random();


        return userIds.SelectMany(
                user => Enumerable.Range(0, 10).Select(
                    _ => CreateMessage(user, chat, MessageStatus.Read, DateTime.UtcNow.AddDays(random.Next(-7, 0)))))
            .ToList();
    }
}