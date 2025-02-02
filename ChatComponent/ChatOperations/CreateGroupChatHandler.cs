﻿using Database.Models;

namespace ChatComponent.ChatOperations;
public record CreateGroupChatCommand(UserId Creator, ChatName Name, ChatDescription Description, ChatImage Image, GroupChatJoinMode JoinMode, ImmutableList<UserId> WithUsers) : IRequest<GroupChat>;

public class CreateGroupChatHandler(AppDbContext context)
    : IRequestHandler<CreateGroupChatCommand, GroupChat>
{
    public async Task<GroupChat> Handle(CreateGroupChatCommand request, CancellationToken cancellationToken)
    {
        var chat = GroupChat.Create(request.Name, request.Description, request.Image, request.JoinMode);
        context.GroupChats.Add(chat);
        var now = DateTime.UtcNow;
        if (request.WithUsers.Contains(request.Creator))
        {
            throw new InvalidOperationException("Creator cannot be in the chat");
        }
        context.AddRange(request.WithUsers.Select(userId => new GroupChatUser(chat.Id, userId, GroupChatRole.User, now)));
        await context.SaveChangesAsync(cancellationToken);
        return chat;
    }
}