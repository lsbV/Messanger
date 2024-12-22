namespace ChatComponent.ChatOperations.Get;

public record GetChatByIdQuery(ChatId Id) : IRequest<Chat>;

public class GetChatByIdHandler(AppDbContext context)
    : IRequestHandler<GetChatByIdQuery, Chat>
{
    public async Task<Chat> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats.FindAsync([request.Id], cancellationToken: cancellationToken);

        switch (chat)
        {
            case null:
                throw new ChatNotFoundException(request.Id);
            case GroupChat groupChat:
                await context.Entry(groupChat)
                    .Collection(gc => gc.Users)
                    .LoadAsync(cancellationToken);
                break;
            case PrivateChat privateChat:
                var user1 = context.Entry(privateChat)
                    .Reference(pc => pc.User1)
                    .LoadAsync(cancellationToken);
                var user2 = context.Entry(privateChat)
                    .Reference(pc => pc.User2)
                    .LoadAsync(cancellationToken);
                await Task.WhenAll(user1, user2);
                break;
        }

        return chat;
    }
}