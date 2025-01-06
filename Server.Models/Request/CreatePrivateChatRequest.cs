using Server.Models.Dtos;

namespace Server.Models.Request;

public record CreatePrivateChatRequest(Guid UserId, MessageContentDto Message);