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

public record Password(PasswordHash Hash, PasswordSalt Salt);
public record PasswordSalt(string Value);
public record PasswordHash(string Value);
public record Avatar(string Url);

public record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static explicit operator string(UserId userId) => userId.Value.ToString();
}

public record UserName(string Value);

public record Email(string Value);

public record AuthorizationVersion(int Value)
{
    public static AuthorizationVersion Default => new(0);
    public AuthorizationVersion Next() => new(this.Value + 1);
}