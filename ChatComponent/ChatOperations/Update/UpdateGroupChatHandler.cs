using Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChatComponent.ChatOperations.Update;

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
        var events = GetEvents(groupChat, request);

        context.Update(groupChat).CurrentValues.SetValues(newChat);
        context.ChatEvents.AddRange(events);
        await context.SaveChangesAsync(cancellationToken);
        return groupChat;
    }

    private static List<ChatEvent> GetEvents(GroupChat existedGroupChat, UpdateGroupChatCommand updateGroupChat)
    {
        var events = new List<ChatEvent>();
        var now = DateTime.UtcNow;
        if (existedGroupChat.ChatName != updateGroupChat.ChatName)
        {
            events.Add(new GroupChatNameUpdatedEvent(ChatEventId.New, existedGroupChat.Id, updateGroupChat.ChatName, now));
        }
        if (existedGroupChat.ChatDescription != updateGroupChat.ChatDescription)
        {
            events.Add(new GroupChatDescriptionUpdatedEvent(ChatEventId.New, existedGroupChat.Id, updateGroupChat.ChatDescription, now));
        }
        if (existedGroupChat.ChatImage != updateGroupChat.ChatImage)
        {
            events.Add(new GroupChatImageUpdatedEvent(ChatEventId.New, existedGroupChat.Id, updateGroupChat.ChatImage, now));
        }
        return events;
    }
}

public class GroupChatNotFoundException(ChatId chatId)
    : EntityNotFoundException<ChatId>(chatId, nameof(GroupChat), $"Group chat with id {chatId} not found.");