namespace MessageComponent.MessageOperations;

public record SendMessageRequest(UserId SenderId, ChatId ReceiverId, MessageContent Content) : IRequest<Message>;

public class SendMessageHandler(IMongoCollection<Message> messageCollection, AppDbContext context)
{
    public async Task<Message> HandleAsync(SendMessageRequest request, CancellationToken cancellationToken)
    {
        var result = await context.CheckUserAndChatExistenceAsync(request.SenderId, request.ReceiverId, cancellationToken);
        if (!result.ChatExists || !result.UserExists || !result.IsMemberOfChat)
        {
            throw new IncorrectMessageRequestException(request.SenderId, request.ReceiverId);
        }

        var message = new Message(
            new MessageId(Guid.NewGuid()),
            request.SenderId,
            request.ReceiverId,
            request.Content,
            MessageStatus.Sent,
            DateTime.UtcNow,
            null
        );

        await messageCollection.InsertOneAsync(message, cancellationToken: cancellationToken);

        return message;
    }
}

public class IncorrectMessageRequestException(UserId senderId, ChatId receiverId)
    : Exception($"User with id {senderId} or chat with id {receiverId} does not exist.");