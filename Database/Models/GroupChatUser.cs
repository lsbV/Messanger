namespace Database.Models;

public record GroupChatUser(ChatId GroupChatId, UserId UserId, GroupChatRole Role, DateTime JoinDate);