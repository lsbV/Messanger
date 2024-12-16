namespace Core;

public record User(UserId Id, UserName Name, Email Email, AuthorizationVersion AuthorizationVersion)
{
    private readonly List<GroupChat> _groupChats = [];
    private readonly List<PrivateChat> _privateChats = [];
    public IReadOnlyList<GroupChat> GroupChats => _groupChats;
    public IReadOnlyList<PrivateChat> PrivateChats => _privateChats;
    public static User Create(UserId id, UserName name, Email email, AuthorizationVersion authorizationVersion)
    {
        return new User(id, name, email, authorizationVersion);
    }


}

public record UserId(Guid Value)
{
    public static UserId Of(Guid value)
    {
        return new UserId(value);
    }

}

public record UserName(string Value)
{
    public static UserName Of(string value)
    {
        return new UserName(value);
    }
}

public record Email(string Value)
{
    public static Email Of(string value)
    {
        return new Email(value);
    }
}

