namespace ChatComponent;

public record CreatePrivateChatCommand(UserId Creator, UserId With) : IRequest<PrivateChat>;


public class CreatePrivateChatHandler(AppDbContext context)
    : IRequestHandler<CreatePrivateChatCommand, Chat>
{
    public async Task<Chat> Handle(CreatePrivateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = PrivateChat.Create(ChatId.Of(Guid.NewGuid()), request.Creator, request.With);
        context.Chats.Add(chat);
        await context.SaveChangesAsync(cancellationToken);
        return chat;
    }
}