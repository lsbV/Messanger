namespace Core;

public record User(
    UserId Id,
    UserName Name,
    Email Email,
    Avatar Avatar,
    AuthorizationVersion AuthorizationVersion)
{
    private readonly List<GroupChat> _groupChats = [];
    private readonly List<PrivateChat> _privateChats = [];
    public IReadOnlyList<GroupChat> GroupChats => _groupChats;
    public IReadOnlyList<PrivateChat> PrivateChats => _privateChats;
}

public record Avatar(string Url);

public record UserId(Guid Value);

public record UserName(string Value);

public record Email(string Value);

