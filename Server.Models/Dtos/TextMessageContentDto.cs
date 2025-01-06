namespace Server.Models.Dtos;

public record TextMessageContentDto(string Type, string Text) : MessageContentDto(Type);