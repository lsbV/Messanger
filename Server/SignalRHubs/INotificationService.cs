namespace Server.SignalRHubs;

public interface INotificationService
{
    Task SendMessage(Message message, IEnumerable<UserId> users);
    Task NotifyAboutUpdateMessage(Message message, IEnumerable<UserId> users);
    Task NotifyAboutChatEvent(ChatEvent chatEvent, IEnumerable<UserId> users);
    Task NotifyAboutChatEvents(IEnumerable<ChatEvent> chatEvents, IEnumerable<UserId> users);
    Task NotifyAboutDeleteMessage(Message message, IEnumerable<UserId> members);
}