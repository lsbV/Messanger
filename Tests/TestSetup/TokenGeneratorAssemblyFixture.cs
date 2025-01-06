using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

[assembly: AssemblyFixture(typeof(TokenGeneratorAssemblyFixture))]

namespace Tests.TestSetup;

public class TokenGeneratorAssemblyFixture
{
    public SigningCredentials SigningCredentials { get; }
    public IOptions<TokenGeneratorOptions> Options { get; }
    public ITokenGenerator TokenGenerator { get; }


    public TokenGeneratorAssemblyFixture()
    {
        var key = new SymmetricSecurityKey("averylongsupersecretkeythatshouldbeatleast16bytes"u8.ToArray());
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        this.Options = Microsoft.Extensions.Options.Options.Create(new TokenGeneratorOptions()
        {
            Audience = "https://localhost:5001/",
            Issuer = "https://localhost:5001/",
            ExpirationTimeInMinutes = 60
        });

        TokenGenerator = new TokenGenerator(SigningCredentials, Options);
    }
}