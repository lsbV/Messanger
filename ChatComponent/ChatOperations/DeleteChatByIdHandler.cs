namespace ChatComponent.ChatOperations;

public record DeleteChatByIdCommand(ChatId Id) : IRequest<Unit>;

public class DeleteChatByIdHandler(AppDbContext context, IMongoCollection<Message> messageCollection)
    : IRequestHandler<DeleteChatByIdCommand, Unit>
{
    public async Task<Unit> Handle(DeleteChatByIdCommand request, CancellationToken cancellationToken)
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
            return Unit.Value;
        }
        catch (Exception)
        {
            await session.RollbackAsync(cancellationToken);
            throw;
        }
    }
}