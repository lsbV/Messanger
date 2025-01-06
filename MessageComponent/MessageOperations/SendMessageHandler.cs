namespace MessageComponent.MessageOperations;

public record SendMessageCommand(UserId SenderId, ChatId ReceiverId, MessageContent Content) : IRequest<Message>;

public class SendMessageHandler(IMongoCollection<Message> messageCollection, AppDbContext context)
{
    public async Task<Message> HandleAsync(SendMessageCommand command, CancellationToken cancellationToken)
    {
        var result = await context.CheckUserAndChatExistenceAsync(command.SenderId, command.ReceiverId, cancellationToken);
        if (!result.ChatExists || !result.UserExists || !result.IsMemberOfChat)
        {
            throw new IncorrectMessageRequestException(command.SenderId, command.ReceiverId);
        }

        var message = new Message(
            new MessageId(Guid.NewGuid()),
            command.SenderId,
            command.ReceiverId,
            command.Content,
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