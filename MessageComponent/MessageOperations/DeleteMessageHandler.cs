using Core.Exceptions;

namespace MessageComponent.MessageOperations;

public record DeleteMessageCommand(UserId DeleterId, MessageId MessageId) : IRequest<Message>;

public class DeleteMessageHandler(IMongoCollection<Message> messageCollection, AppDbContext context)
        : IRequestHandler<DeleteMessageCommand, Message>
{
    public async Task<Message> Handle(DeleteMessageCommand command, CancellationToken cancellationToken)
    {
        var message = await messageCollection.Find(m => m.Id == command.MessageId).FirstOrDefaultAsync(cancellationToken);
        if (message is null)
            throw new MessageNotFoundException(command.MessageId);

        if (message.SenderId != command.DeleterId)
        {
            var groupChatUser = await context.GroupChatUsers.FindAsync([message.RecipientId, command.DeleterId], cancellationToken);

            if (groupChatUser?.Role is not (GroupChatRole.Manager or GroupChatRole.Owner))
                throw new ForbiddenOperationException(nameof(DeleteMessageCommand), command.DeleterId);
        }

        message = message with { Content = new DeletedContent(DateTime.UtcNow), EditedAt = DateTime.UtcNow };
        await messageCollection.FindOneAndReplaceAsync(m => m.Id == command.MessageId, message, cancellationToken: cancellationToken);

        return message;
    }
}
