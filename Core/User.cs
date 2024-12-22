namespace Core;

public record User(
    UserId Id,
    UserName Name,
    Password Password,
    Email Email,
    Avatar Avatar,
    AuthorizationVersion AuthorizationVersion)
{
    private readonly List<GroupChat> _groupChats = [];
    private readonly List<PrivateChat> _privateChats = [];
    public IReadOnlyList<GroupChat> GroupChats => _groupChats;
    public IReadOnlyList<PrivateChat> PrivateChats => _privateChats;
}

public record PasswordSalt(string Value);

public record Password(string Hash, string Salt);

public record Avatar(string Url);

public record UserId(Guid Value);

public record UserName(string Value);

public record Email(string Value);

public record AuthorizationVersion(int Value)
{
    public static AuthorizationVersion Default => new(0);
    public AuthorizationVersion Next() => new(this.Value + 1);
}