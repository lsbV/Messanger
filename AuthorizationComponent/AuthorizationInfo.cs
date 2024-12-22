namespace AuthorizationComponent;

public record AuthorizationInfo(
    UserId UserId,
    UserName Name,
    Email Email,
    Avatar Avatar,
    string Token
);