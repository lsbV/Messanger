using Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChatComponent.ChatOperations;

public record UpdateGroupChatCommand(ChatId ChatId, ChatName ChatName, ChatDescription ChatDescription, ChatImage ChatImage) : IRequest<GroupChat>;

public class UpdateGroupChatHandler(AppDbContext context) : IRequestHandler<UpdateGroupChatCommand, GroupChat>
{
    public async Task<GroupChat> Handle(UpdateGroupChatCommand request, CancellationToken cancellationToken)
    {
        var groupChat = await context.GroupChats
            .FirstOrDefaultAsync(g => g.Id == request.ChatId, cancellationToken);
        if (groupChat is null)
        {
            throw new GroupChatNotFoundException(request.ChatId);
        }

        var newChat = groupChat with
        {
            ChatName = request.ChatName,
            ChatDescription = request.ChatDescription,
            ChatImage = request.ChatImage
        };

        context.Update(groupChat).CurrentValues.SetValues(newChat);
        await context.SaveChangesAsync(cancellationToken);
        return groupChat;
    }
}

public class GroupChatNotFoundException(ChatId chatId)
    : EntityNotFoundException<ChatId>(chatId, nameof(GroupChat), $"Group chat with id {chatId} not found.");