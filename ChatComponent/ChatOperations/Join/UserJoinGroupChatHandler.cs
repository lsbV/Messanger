using Core.Exceptions;
using Database.Models;

namespace ChatComponent.ChatOperations.Join;

public record UserJoinGroupChatRequest(UserId UserId, ChatId GroupChatId) : IRequest;

public class UserJoinGroupChatHandler(AppDbContext context)
    : IRequestHandler<UserJoinGroupChatRequest>
{
    public async Task Handle(UserJoinGroupChatRequest request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats.FindAsync([request.GroupChatId], cancellationToken);

        if (chat is not GroupChat groupChat)
        {
            if (chat is null)
                throw new ChatNotFoundException(request.GroupChatId);
            else
                throw new ForbiddenOperationException(nameof(UserJoinGroupChatRequest), request.UserId);
        }
        if (groupChat.JoinMode is not GroupChatJoinMode.Free)
            throw new ForbiddenOperationException(nameof(UserJoinGroupChatRequest), request.UserId);

        var groupChatUser = await context.GroupChatUsers.FindAsync([request.GroupChatId, request.UserId], cancellationToken);
        if (groupChatUser is not null)
            throw new UserAlreadyInGroupChatException(request.UserId, request.GroupChatId);

        var now = DateTime.UtcNow;
        groupChatUser = new GroupChatUser(groupChat.Id, request.UserId, GroupChatRole.User, now);
        var joinEvent = new UserJoinedGroupChatEvent(ChatEventId.New, groupChat.Id, request.UserId, now);
        context.Add(groupChatUser);
        context.Add(joinEvent);


        await context.SaveChangesAsync(cancellationToken);
    }
}