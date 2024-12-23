using Core.Exceptions;

namespace MessageComponent.MessageOperations;

public record EditMessageRequest(MessageId MessageId, MessageContent NewContent) : IRequest<Message>;

public class EditMessageHandler(IMongoCollection<Message> messageCollection)
    : IRequestHandler<EditMessageRequest, Message>
{
    public async Task<Message> Handle(EditMessageRequest request, CancellationToken cancellationToken)
    {
        var message = await messageCollection.Find(m => m.Id == request.MessageId).FirstOrDefaultAsync(cancellationToken);

        if (message == null)
            throw new MessageNotFoundException(request.MessageId);
        if (message.Content == request.NewContent)
            return message;

        message = message with { Content = request.NewContent, EditedAt = DateTime.UtcNow };
        await messageCollection.ReplaceOneAsync(m => m.Id == request.MessageId, message, cancellationToken: cancellationToken);
        return message;
    }
}

public class MessageNotFoundException(MessageId id) : EntityNotFoundException<MessageId>(id, nameof(Message)) { }
