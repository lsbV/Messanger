using Core.Exceptions;
using Database.Models;

namespace ChatComponent.ChatOperations;

public record JoinGroupChatCommand(UserId UserId, ChatId GroupChatId) : IRequest<GroupChatUser>;

public class UserJoinGroupChatHandler(AppDbContext context)
    : IRequestHandler<JoinGroupChatCommand, GroupChatUser>
{
    public async Task<GroupChatUser> Handle(JoinGroupChatCommand command, CancellationToken cancellationToken)
    {
        var chat = await context.Chats.FindAsync([command.GroupChatId], cancellationToken);

        if (chat is not GroupChat groupChat)
        {
            if (chat is null)
                throw new ChatNotFoundException(command.GroupChatId);
            else
                throw new ForbiddenOperationException(nameof(JoinGroupChatCommand), command.UserId);
        }
        if (groupChat.JoinMode is not GroupChatJoinMode.Free)
            throw new ForbiddenOperationException(nameof(JoinGroupChatCommand), command.UserId);

        var groupChatUser = await context.GroupChatUsers.FindAsync([command.GroupChatId, command.UserId], cancellationToken);
        if (groupChatUser is not null)
            throw new UserAlreadyInGroupChatException(command.UserId, command.GroupChatId);

        groupChatUser = new GroupChatUser(groupChat.Id, command.UserId, GroupChatRole.User, DateTime.UtcNow);
        context.Add(groupChatUser);

        await context.SaveChangesAsync(cancellationToken);

        return groupChatUser;
    }
}