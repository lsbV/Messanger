internal record MessageDto(
    string Id,
    string SenderId,
    string RecipientId,
    ContentDto Content,
    string Status,
    string CreatedAt,
    string? EditedAt
);