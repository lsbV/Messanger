namespace Core;

public record User(UserId Id, UserName Name, Email Email, Role Role, AuthorizationVersion AuthorizationVersion)
{
    public static User Create(UserId id, UserName name, Email email, Role role, AuthorizationVersion authorizationVersion)
    {
        return new User(id, name, email, role, authorizationVersion);
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

public record Role(string Value)
{
    public static Role Of(string value)
    {
        return new Role(value);
    }
}