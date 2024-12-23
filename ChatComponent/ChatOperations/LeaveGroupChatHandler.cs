using Database.Models;

namespace ChatComponent.ChatOperations;

public record LeaveGroupChatCommand(ChatId ChatId, UserId UserId) : IRequest;

public class LeaveGroupChatHandler(AppDbContext context) : IRequestHandler<LeaveGroupChatCommand>
{

    public async Task Handle(LeaveGroupChatCommand request, CancellationToken cancellationToken)
    {
        var chatUser = await context.GroupChatUsers.FindAsync([request.ChatId, request.UserId], cancellationToken);
        if (chatUser is null)
        {
            throw new GroupChatUserNotFoundException(new GroupChatUser(request.ChatId, request.UserId, GroupChatRole.User, DateTime.UtcNow));
        }
        if (chatUser.GroupChatRole == GroupChatRole.Owner)
        {
            throw new OwnerCannotLeaveGroupChatException(chatUser);
        }

        context.GroupChatUsers.Remove(chatUser);
        var leaveEvent = new UserLeftGroupChatEvent(ChatEventId.New, request.ChatId, request.UserId, DateTime.UtcNow);
        context.ChatEvents.Add(leaveEvent);
        await context.SaveChangesAsync(cancellationToken);
    }
}