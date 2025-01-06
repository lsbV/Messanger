using Microsoft.AspNetCore.SignalR;

namespace Server.SignalRHubs;

public class SignalRNotificationService(IHubContext<NotificationHub> hub) : INotificationService
{
    public async Task SendMessage(Message message, IEnumerable<UserId> users)
    {
        await hub.Clients.Users(users.Select(u => u.Value.ToString()))
            .SendAsync("ReceiveMessage", message);
    }

    public async Task NotifyAboutUpdateMessage(Message message, IEnumerable<UserId> users)
    {
        await hub.Clients.Users(users.Select(u => u.Value.ToString()))
            .SendAsync("ReceiveEditedMessage", message);
    }

    public async Task NotifyAboutChatEvent(ChatEvent chatEvent, IEnumerable<UserId> users)
    {
        await hub.Clients.Users(users.Select(u => u.Value.ToString()))
            .SendAsync("ReceiveChatEvent", chatEvent);
    }

    public async Task NotifyAboutChatEvents(IEnumerable<ChatEvent> chatEvents, IEnumerable<UserId> users)
    {
        await hub.Clients.Users(users.Select(u => u.Value.ToString()))
            .SendAsync("ReceiveChatEvents", chatEvents);
    }

    public async Task NotifyAboutDeleteMessage(Message message, IEnumerable<UserId> members)
    {
        await hub.Clients.Users(members.Select(u => u.Value.ToString()))
            .SendAsync("ReceiveDeletedMessage", message);
    }
}