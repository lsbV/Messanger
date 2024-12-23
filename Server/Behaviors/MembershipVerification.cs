using Microsoft.EntityFrameworkCore;

namespace Server.Behaviors;

public record VerifiedGetChatByIdQuery(UserId RequesterId, ChatId ChatId) : GetChatByIdQuery(ChatId), IVerifiedRequestForChat;

public record VerifiedUpdateGroupChatCommand(
    UserId RequesterId,
    ChatId ChatId,
    ChatName Name,
    ChatDescription Description,
    ChatImage Image)
    : UpdateGroupChatCommand(ChatId, Name, Description, Image), IVerifiedRequestForChat;

public record VerifiedDeleteChatByIdCommand(UserId RequesterId, ChatId ChatId) : DeleteChatByIdCommand(ChatId), IVerifiedRequestForChat;

internal interface IVerifiedRequestForChat
{
    UserId RequesterId { get; }
    ChatId ChatId { get; }
}

public class MembershipVerification(AppDbContext context)
    : IPipelineBehavior<GetChatByIdQuery, Chat>,
        IPipelineBehavior<UpdateGroupChatCommand, GroupChat>,
        IPipelineBehavior<DeleteChatByIdCommand, Unit>
{
    public async Task<Chat> Handle(GetChatByIdQuery request, RequestHandlerDelegate<Chat> next, CancellationToken cancellationToken)
    {
        await Verify(request, cancellationToken);

        return await next();
    }

    public async Task<GroupChat> Handle(UpdateGroupChatCommand request, RequestHandlerDelegate<GroupChat> next, CancellationToken cancellationToken)
    {
        await Verify(request, cancellationToken, GroupChatRole.Owner, GroupChatRole.Manager);

        return await next();
    }
    public async Task<Unit> Handle(DeleteChatByIdCommand request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken)
    {
        await Verify(request, cancellationToken, GroupChatRole.Owner);

        return await next();
    }




    private async Task Verify(object request, CancellationToken cancellationToken, params GroupChatRole[] allowedRoles)
    {
        var verification = await Verify(request, cancellationToken);

        var groupChatUser = await context.GroupChatUsers.FirstOrDefaultAsync(g => g.UserId == verification.RequesterId && g.GroupChatId == verification.ChatId, cancellationToken);
        if (groupChatUser is null || !allowedRoles.Contains(groupChatUser.GroupChatRole))
        {
            throw new ForbiddenOperationException(nameof(GetChatByIdQuery), verification.RequesterId);
        }
    }
    private async Task<IVerifiedRequestForChat> Verify(object request, CancellationToken cancellationToken)
    {
        if (request is not IVerifiedRequestForChat verifiedRequest)
        {
            throw new ArgumentException("Request is not a verified request", nameof(request));
        }
        var verification = await context.CheckUserAndChatExistenceAsync(verifiedRequest.RequesterId, verifiedRequest.ChatId, cancellationToken);
        if (!verification.UserExists || !verification.ChatExists || !verification.IsMemberOfChat)
        {
            throw new ForbiddenOperationException(request.GetType().Name, verifiedRequest.RequesterId);
        }
        return verifiedRequest;
    }


}