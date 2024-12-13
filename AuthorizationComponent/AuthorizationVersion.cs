namespace AuthorizationComponent;

public record AuthorizationVersion(int Number)
{
    public virtual bool IsDefault => this == Default;
    public virtual AuthorizationVersion Next => new(Number + 1);
    public static AuthorizationVersion Default => new(0);
    public override string ToString()
    {
        return Number.ToString();
    }
}