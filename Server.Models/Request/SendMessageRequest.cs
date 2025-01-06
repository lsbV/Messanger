using Server.Models.Dtos;

namespace Server.Models.Request;

public record SendMessageRequest(Guid ChatId, MessageContentDto Content);