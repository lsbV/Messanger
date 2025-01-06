using Server.Models.Dtos;

namespace Server.Models.Request;

public record EditMessageRequest(Guid MessageId, MessageContentDto Content);