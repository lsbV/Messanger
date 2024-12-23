namespace MessageComponent.MessageOperations;

public record GetMessagesByChatIdRequest(ChatId ChatId, UserId RequesterId, DateTime After, int Count) : IRequest<List<Message>>;

public class GetMessagesByChatIdHandler(IMongoCollection<Message> messageCollection)
    : IRequestHandler<GetMessagesByChatIdRequest, List<Message>>
{
    public async Task<List<Message>> Handle(GetMessagesByChatIdRequest request, CancellationToken cancellationToken)
    {
        var messages = await messageCollection.Find(m => m.RecipientId == request.ChatId && (m.CreatedAt > request.After || m.EditedAt > request.After))
            .SortByDescending(m => m.CreatedAt)
            .Limit(request.Count)
            .ToListAsync(cancellationToken);
        return messages;
    }
}