namespace Tests.AuthorizationComponent;

public class TokenGeneratorTests(TokenGeneratorAssemblyFixture fixture)
{
    private readonly TokenGenerator _generator = new TokenGenerator(fixture.SigningCredentials, fixture.Options);

    [Fact]
    public void GenerateToken_ReturnToken()
    {
        var user = EntityFactory.CreateRandomUser();

        var result = _generator.GenerateToken(user);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateToken_ReturnSameTokenForSameUser()
    {
        var user = EntityFactory.CreateRandomUser();

        var result1 = _generator.GenerateToken(user);
        var result2 = _generator.GenerateToken(user);

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEmpty(result1);
        Assert.NotEmpty(result2);
        Assert.Equal(result1, result2);
    }

}