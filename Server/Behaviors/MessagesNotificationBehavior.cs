using MessageComponent.MessageOperations;
using Server.SignalRHubs;

namespace Server.Behaviors;

public class MessagesNotificationBehavior(INotificationService notificationService, AppDbContext context) :
    IPipelineBehavior<SendMessageCommand, Message>,
    IPipelineBehavior<EditMessageCommand, Message>
{
    public async Task<Message> Handle(SendMessageCommand request, RequestHandlerDelegate<Message> next, CancellationToken cancellationToken)
    {
        var message = await next();
        var members = await context.GetChatMembersAsync(message.RecipientId, cancellationToken);

        await notificationService.SendMessage(message, members);
        return message;
    }

    public async Task<Message> Handle(EditMessageCommand request, RequestHandlerDelegate<Message> next, CancellationToken cancellationToken)
    {
        var message = await next();
        var members = await context.GetChatMembersAsync(message.RecipientId, cancellationToken);

        await notificationService.NotifyAboutUpdateMessage(message, members);
        return message;
    }

    public async Task<Message> Handle(DeleteMessageCommand request, RequestHandlerDelegate<Message> next, CancellationToken cancellationToken)
    {
        var message = await next();
        var members = await context.GetChatMembersAsync(message.RecipientId, cancellationToken);

        await notificationService.NotifyAboutDeleteMessage(message, members);
        return message;
    }
}