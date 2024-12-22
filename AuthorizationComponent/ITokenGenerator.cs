namespace AuthorizationComponent;

public interface ITokenGenerator
{
    string GenerateToken(User user);
}