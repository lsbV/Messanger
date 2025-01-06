namespace AuthorizationComponent;

public class TokenGenerator(SigningCredentials credentials, IOptions<TokenGeneratorOptions> options) : ITokenGenerator
{
    private readonly TokenGeneratorOptions _options = options.Value;

    public string GenerateToken(User user)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new(ClaimTypes.Name, user.Name.Value),
            new(ClaimTypes.Email, user.Email.Value),
            new(ClaimTypes.Version, user.AuthorizationVersion.Value.ToString())
        ];

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMinutes(_options.ExpirationTimeInMinutes), // remove seconds to avoid token expiration issues
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}