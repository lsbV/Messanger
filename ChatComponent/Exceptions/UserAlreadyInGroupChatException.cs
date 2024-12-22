using Core.Exceptions;
using Database.Models;

namespace ChatComponent.Exceptions;

public class UserAlreadyInGroupChatException : EntityAlreadyExistsException
{
    public UserAlreadyInGroupChatException(UserId requestUserId, ChatId requestGroupChatId) : base(nameof(GroupChatUser), nameof(GroupChatUser.UserId), requestUserId.ToString())
    {
    }
}