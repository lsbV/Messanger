namespace Server.Models.Request;

public record CreateGroupChatRequest(string Name, string Description, string ImageUrl, string JoinMode, Guid[] Members);