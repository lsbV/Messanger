using ChatComponent.ChatOperations;
using Core.Exceptions;
using Database.Models;

namespace ChatComponent.Exceptions;

public class OwnerCannotLeaveGroupChatException(GroupChatUser chatUser) : ForbiddenOperationException(
    nameof(LeaveGroupChatHandler), chatUser.UserId,
    $"Owner of group chat with id {chatUser.GroupChatId} cannot leave the chat")
{
    public readonly ChatId ChatId = chatUser.GroupChatId;
}