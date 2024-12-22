namespace AuthorizationComponent;

public class AuthorizationService(AppDbContext context, ITokenGenerator tokenGenerator) : IAuthorizationService
{
    public async Task RegisterUserAsync(User user)
    {
        var salt = GenerateSalt();
        user = user with { Password = new Password(HashPassword(user.Password.Hash, salt), salt) };

        context.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task<User> GetUserAsync(UserId userId, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([userId], cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }
        return user;
    }

    public async Task<User> GetUserByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(new UserId(Guid.Empty));
        }
        return user;
    }

    private async Task<bool> ValidateUserPasswordAsync(UserId userId, string password, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId, cancellationToken);
        return user.Password.Hash == HashPassword(password, user.Password.Salt);
    }

    public async Task<AuthorizationInfo> LoginUserAsync(Email email, string password, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is null || !await ValidateUserPasswordAsync(user.Id, password, cancellationToken))
        {
            throw new IncorrectCredentialException(email);
        }
        var token = tokenGenerator.GenerateToken(user);
        return new AuthorizationInfo(user.Id, user.Name, user.Email, user.Avatar, token);
    }

    public async Task ChangePasswordAsync(UserId userId, AuthorizationVersion authorizationVersion, string newPassword, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId, cancellationToken);
        if (!await ValidateAuthorizationVersionAsync(userId, authorizationVersion, cancellationToken))
        {
            throw new ForbiddenOperationException(nameof(ChangePasswordAsync), userId);
        }
        var salt = GenerateSalt();
        var newUser = user with
        {
            Password = new Password(HashPassword(newPassword, salt), salt),
            AuthorizationVersion = user.AuthorizationVersion.Next()
        };
        context.Update(user).CurrentValues.SetValues(newUser);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ValidateAuthorizationVersionAsync(UserId userId, AuthorizationVersion authorizationVersion, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId, cancellationToken);
        return user.AuthorizationVersion == authorizationVersion;
    }

    public static string HashPassword(string password, string salt)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + salt));
        return Convert.ToHexStringLower(hashedBytes);
    }

    public static string GenerateSalt()
    {
        var salt = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return Convert.ToHexStringLower(salt);
    }
}