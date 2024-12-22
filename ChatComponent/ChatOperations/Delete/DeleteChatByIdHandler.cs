namespace ChatComponent.ChatOperations.Delete;

public record DeleteChatByIdCommand(ChatId Id) : IRequest;

public class DeleteChatByIdHandler(AppDbContext context, IMongoCollection<Message> messageCollection)
    : IRequestHandler<DeleteChatByIdCommand>
{
    public async Task Handle(DeleteChatByIdCommand request, CancellationToken cancellationToken)
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
        }
        catch (Exception)
        {
            await session.RollbackAsync(cancellationToken);
            throw;
        }
    }
}