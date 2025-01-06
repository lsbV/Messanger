using Core.Exceptions;

namespace MessageComponent.MessageOperations;

public record EditMessageCommand(UserId EditorId, MessageId MessageId, MessageContent NewContent) : IRequest<Message>;

public class EditMessageHandler(IMongoCollection<Message> messageCollection)
    : IRequestHandler<EditMessageCommand, Message>
{
    public async Task<Message> Handle(EditMessageCommand command, CancellationToken cancellationToken)
    {
        var message = await messageCollection.Find(m => m.Id == command.MessageId).FirstOrDefaultAsync(cancellationToken);

        if (message == null)
            throw new MessageNotFoundException(command.MessageId);
        if (message.SenderId != command.EditorId)
            throw new ForbiddenOperationException(nameof(EditMessageCommand), command.EditorId);
        if (message.Content == command.NewContent)
            return message;

        message = message with { Content = command.NewContent, EditedAt = DateTime.UtcNow };
        await messageCollection.ReplaceOneAsync(m => m.Id == command.MessageId, message, cancellationToken: cancellationToken);
        return message;
    }
}

public class MessageNotFoundException(MessageId id) : EntityNotFoundException<MessageId>(id, nameof(Message)) { }