namespace Core;

public record User(UserId Id, UserName Name, Email Email)
{
    public static User Create(UserId id, UserName name, Email email)
    {
        return new User(id, name, email);
    }
}

public record UserId(Guid Value)
{
    public static Guid Of(Guid value)
    {
        return value;
    }

}

public record UserName(string Value)
{
    public static string Of(string value)
    {
        return value;
    }
}

public record Email(string Value)
{
    public static string Of(string value)
    {
        return value;
    }
}