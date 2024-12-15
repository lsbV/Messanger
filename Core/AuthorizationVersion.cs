namespace Core;

public record AuthorizationVersion(int Value)
{
    public virtual bool IsDefault => this == Default;
    public virtual AuthorizationVersion Next => new(Value + 1);
    public static AuthorizationVersion Default => new(0);
    public override string ToString()
    {
        return Value.ToString();
    }

    public static AuthorizationVersion Of(int value)
    {
        return new AuthorizationVersion(value);
    }
}