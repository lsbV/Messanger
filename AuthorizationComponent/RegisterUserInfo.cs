namespace AuthorizationComponent;

public record RegisterUserInfo(UserName Name, Email Email, RawPassword Password, AvatarReadOnlyFileStream AvatarReadOnlyStream);

public record AvatarReadOnlyFileStream(Stream Content);