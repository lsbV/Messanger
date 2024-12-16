namespace ChatComponent;

public record CreatePrivateChatCommand(UserId Creator, UserId With) : IRequest<PrivateChat>;


public class CreatePrivateChatHandler(AppDbContext context)
    : IRequestHandler<CreatePrivateChatCommand, PrivateChat>
{
    public async Task<PrivateChat> Handle(CreatePrivateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = PrivateChat.Create(new ChatId(Guid.NewGuid()), request.Creator, request.With);
        context.PrivateChats.Add(chat);
        await context.SaveChangesAsync(cancellationToken);
        return chat;
    }
}