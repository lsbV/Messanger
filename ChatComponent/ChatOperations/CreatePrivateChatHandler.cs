using Microsoft.EntityFrameworkCore;

namespace ChatComponent.ChatOperations;

public record CreatePrivateChatCommand(UserId Creator, UserId With, MessageContent FirstMessage) : IRequest<PrivateChat>;


public class CreatePrivateChatHandler(AppDbContext context, IMongoCollection<Message> messageCollection)
    : IRequestHandler<CreatePrivateChatCommand, PrivateChat>
{
    public async Task<PrivateChat> Handle(CreatePrivateChatCommand request, CancellationToken cancellationToken)
    {
        var existedChat = await context.PrivateChats.FirstOrDefaultAsync(c =>
            c.UserId1 == request.Creator && c.UserId2 == request.With ||
            c.UserId1 == request.With && c.UserId2 == request.Creator,
            cancellationToken
        );
        if (existedChat != null)
        {
            return existedChat;
        }
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var chat = PrivateChat.Create(new ChatId(Guid.NewGuid()), request.Creator, request.With);
            context.PrivateChats.Add(chat);
            await context.SaveChangesAsync(cancellationToken);

            var message = new Message(
                new MessageId(Guid.NewGuid()),
                request.Creator,
                chat.Id,
                request.FirstMessage,
                MessageStatus.Sent,
                DateTime.UtcNow,
                null
            );
            await messageCollection.InsertOneAsync(message, cancellationToken: cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return chat;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}