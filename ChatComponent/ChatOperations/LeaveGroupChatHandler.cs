using Database.Models;

namespace ChatComponent.ChatOperations;

public record LeaveGroupChatCommand(ChatId ChatId, UserId UserId) : IRequest<GroupChatUser>;

public class LeaveGroupChatHandler(AppDbContext context) : IRequestHandler<LeaveGroupChatCommand, GroupChatUser>
{
    public async Task<GroupChatUser> Handle(LeaveGroupChatCommand request, CancellationToken cancellationToken)
    {
        var chatUser = await context.GroupChatUsers.FindAsync([request.ChatId, request.UserId], cancellationToken);
        if (chatUser is null)
        {
            throw new GroupChatUserNotFoundException(new GroupChatUser(request.ChatId, request.UserId, GroupChatRole.User, DateTime.UtcNow));
        }
        if (chatUser.Role == GroupChatRole.Owner)
        {
            throw new OwnerCannotLeaveGroupChatException(chatUser);
        }

        context.GroupChatUsers.Remove(chatUser);
        await context.SaveChangesAsync(cancellationToken);
        return chatUser;
    }
}