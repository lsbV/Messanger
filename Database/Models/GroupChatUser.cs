namespace Database.Models;

public record GroupChatUser(ChatId GroupChatId, UserId UserId, GroupChatRole GroupChatRole, DateTime JoinDate)
{
    private GroupChatUser() : this(default!, default!, default!, default!) { }

}