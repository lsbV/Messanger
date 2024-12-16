using Database.Configurations;

namespace ChatComponent;
public record CreateGroupChatCommand(UserId Creator, ChatName Name, ImmutableList<UserId> WithUsers) : IRequest<GroupChat>;

public class CreateGroupChatHandler(AppDbContext context)
    : IRequestHandler<CreateGroupChatCommand, GroupChat>
{
    public async Task<GroupChat> Handle(CreateGroupChatCommand request, CancellationToken cancellationToken)
    {
        var chat = GroupChat.Create(new ChatId(Guid.NewGuid()), request.Name);
        context.GroupChats.Add(chat);
        //context.AddRange(request.WithUsers.Select(userId => new GroupChatUser(chat.Id, userId, new Role("User")))); 
        await context.SaveChangesAsync(cancellationToken);
        return chat;
    }
}