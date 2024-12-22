namespace AuthorizationComponent;

public class TokenGeneratorOptions
{
    public required string Issuer { get; init; }
    public required long ExpirationTimeInMinutes { get; init; }
    public required string Audience { get; init; }
}