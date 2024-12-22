using Core.Exceptions;
using Database.Models;

namespace ChatComponent.Exceptions;

public class GroupChatUserNotFoundException(GroupChatUser chatUser) : EntityNotFoundException<GroupChatUser>(chatUser,
    nameof(GroupChatUser),
    $"User with id {chatUser.UserId} not found in group chat with id {chatUser.GroupChatId}");