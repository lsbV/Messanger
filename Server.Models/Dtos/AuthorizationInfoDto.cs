namespace Server.Models.Dtos;

public record AuthorizationInfoDto(
    string Id,
    string Name,
    string Email,
    string Avatar,
    string Token
);