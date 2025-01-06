namespace Server.Models.Dtos;

public record MessageDto(
    string Id,
    string SenderId,
    string RecipientId,
    MessageContentDto MessageContent,
    string Status,
    string CreatedAt,
    string? EditedAt
);