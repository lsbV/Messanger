namespace AuthorizationComponent;

public class TokenGenerator(SigningCredentials credentials, IOptions<TokenGeneratorOptions> options) : ITokenGenerator
{
    private readonly TokenGeneratorOptions options = options.Value;

    public string GenerateToken(User user)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new(ClaimTypes.Name, user.Name.Value),
            new(ClaimTypes.Email, user.Email.Value)
        ];
        var nowWithoutSeconds = DateTime.Now.AddSeconds(-DateTime.Now.Second);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMinutes(options.ExpirationTimeInMinutes), // remove seconds to avoid token expiration issues
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}