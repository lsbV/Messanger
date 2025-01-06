using Database.Models;
using Microsoft.EntityFrameworkCore;
using Server.SignalRHubs;

namespace Server.Behaviors;

public class ChatNotificationBehavior(INotificationService notificationService, AppDbContext context) :
    IPipelineBehavior<CreateGroupChatCommand, GroupChat>,
    IPipelineBehavior<CreatePrivateChatCommand, PrivateChat>,
    IPipelineBehavior<UpdateGroupChatCommand, GroupChat>,
    IPipelineBehavior<JoinGroupChatCommand, GroupChatUser>,
    IPipelineBehavior<LeaveGroupChatCommand, GroupChatUser>
{
    public async Task<GroupChat> Handle(CreateGroupChatCommand request, RequestHandlerDelegate<GroupChat> next, CancellationToken cancellationToken)
    {
        var chat = await next();
        var chatEvent = new UserCreatedGroupChatEvent(ChatEventId.New, chat.Id, request.Creator, DateTime.UtcNow);
        context.Add(chatEvent);

        await notificationService.NotifyAboutChatEvent(chatEvent, request.WithUsers);
        return chat;
    }


    public async Task<PrivateChat> Handle(CreatePrivateChatCommand request, RequestHandlerDelegate<PrivateChat> next, CancellationToken cancellationToken)
    {
        var chat = await next();
        var chatEvent = new UserCreatedPrivateChatEvent(ChatEventId.New, chat.Id, request.Creator, DateTime.UtcNow);
        context.Add(chatEvent);
        await context.SaveChangesAsync(CancellationToken.None);


        await notificationService.NotifyAboutChatEvent(chatEvent, new[] { request.Creator, request.With });
        return chat;
    }


    public async Task<GroupChat> Handle(UpdateGroupChatCommand request, RequestHandlerDelegate<GroupChat> next, CancellationToken cancellationToken)
    {
        var existedGroupChat = await context.GroupChats.FindAsync([request.ChatId], cancellationToken) ?? throw new GroupChatNotFoundException(request.ChatId);
        var groupChat = await next();
        var events = GetEvents(existedGroupChat, request);
        context.AddRange(events);
        await context.SaveChangesAsync(CancellationToken.None);
        var userIds = await context.GroupChatUsers.Where(u => u.GroupChatId == groupChat.Id).Select(u => u.UserId).ToListAsync(cancellationToken);

        await notificationService.NotifyAboutChatEvents(events, userIds);
        return groupChat;
    }

    public async Task<GroupChatUser> Handle(JoinGroupChatCommand command, RequestHandlerDelegate<GroupChatUser> next, CancellationToken cancellationToken)
    {
        var groupChatUser = await next();
        var joinEvent = new UserJoinedGroupChatEvent(ChatEventId.New, groupChatUser.GroupChatId, command.UserId, DateTime.UtcNow);
        context.Add(joinEvent);
        await context.SaveChangesAsync(CancellationToken.None);
        var userIds = await context.GroupChatUsers.Where(u => u.GroupChatId == groupChatUser.GroupChatId).Select(u => u.UserId).ToListAsync(cancellationToken);

        await notificationService.NotifyAboutChatEvent(joinEvent, userIds);
        return groupChatUser;
    }

    public async Task<GroupChatUser> Handle(LeaveGroupChatCommand request, RequestHandlerDelegate<GroupChatUser> next, CancellationToken cancellationToken)
    {
        var chat = await next();
        var leaveEvent = new UserLeftGroupChatEvent(ChatEventId.New, request.ChatId, request.UserId, DateTime.UtcNow);
        context.Add(leaveEvent);
        await context.SaveChangesAsync(CancellationToken.None);
        var userIds = await context.GroupChatUsers.Where(u => u.GroupChatId == chat.GroupChatId).Select(u => u.UserId).ToListAsync(cancellationToken);

        await notificationService.NotifyAboutChatEvent(leaveEvent, userIds);
        return chat;
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