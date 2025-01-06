namespace ChatComponent.ChatOperations;

public record DeleteChatCommand(ChatId Id) : IRequest<Chat>;

public class DeleteChatByIdHandler(AppDbContext context, IMongoCollection<Message> messageCollection)
    : IRequestHandler<DeleteChatCommand, Chat>
{
    public async Task<Chat> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats.FindAsync([request.Id], cancellationToken);
        if (chat == null)
        {
            throw new ChatNotFoundException(request.Id);
        }
        await using var session = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Chats.Remove(chat);

            await context.SaveChangesAsync(cancellationToken);

            await messageCollection.DeleteManyAsync(m => m.RecipientId == chat.Id,
                cancellationToken: cancellationToken);

            await session.CommitAsync(cancellationToken);
            return chat;
        }
        catch (Exception)
        {
            await session.RollbackAsync(cancellationToken);
            throw;
        }
    }
}