using Database.Configurations;

namespace ChatComponent;
public record CreateGroupChatCommand(UserId Creator, ChatName Name, ImmutableList<UserId> WithUsers) : IRequest<GroupChat>;

public class CreateGroupChatHandler(AppDbContext context)
    : IRequestHandler<CreateGroupChatCommand, Chat>
{
    public async Task<Chat> Handle(CreateGroupChatCommand request, CancellationToken cancellationToken)
    {
        var chat = GroupChat.Create(ChatId.Of(Guid.NewGuid()), request.Name, request.Creator);
        context.Chats.Add(chat);
        context.AddRange(request.WithUsers.Select(userId => new GroupChatUser(chat.ChatId, userId)));
        await context.SaveChangesAsync(cancellationToken);
        return chat;
    }
}