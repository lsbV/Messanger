namespace MessageComponent.MessageOperations;

public record DeleteMessageRequest(MessageId MessageId) : IRequest;

public class DeleteMessageHandler(IMongoCollection<Message> messageCollection) : IRequestHandler<DeleteMessageRequest>
{
    public async Task Handle(DeleteMessageRequest request, CancellationToken cancellationToken)
    {
        var result = await messageCollection.DeleteOneAsync(m => m.Id == request.MessageId, cancellationToken: cancellationToken);
        if (result.DeletedCount == 0)
            throw new MessageNotFoundException(request.MessageId);
    }
}