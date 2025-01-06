using Azure.Storage.Blobs;

namespace AuthorizationComponent;

public class AuthorizationService(AppDbContext context, ITokenGenerator tokenGenerator, BlobContainerClient containerClient) : IAuthorizationService
{
    public async Task RegisterUserAsync(RegisterUserInfo registerUserInfo, CancellationToken cancellationToken)
    {
        if (await context.Users.Where(u => u.Email == registerUserInfo.Email).AnyAsync(cancellationToken))
        {
            throw new UserAlreadyExistsException(registerUserInfo.Email);
        }
        var salt = GenerateSalt();
        var password = new Password(HashPassword(registerUserInfo.Password, salt), salt);
        var id = UserId.New();
        var blobName = "avatars/" + id.Value;
        var blobClient = containerClient.GetBlobClient(blobName);
        var user = new User(
            id,
            registerUserInfo.Name,
            password,
            registerUserInfo.Email,
            new Avatar(blobName),
            AuthorizationVersion.Default);

        await blobClient.UploadAsync(registerUserInfo.AvatarReadOnlyStream.Content, false, cancellationToken);
        context.Add(user);
        await context.SaveChangesAsync(cancellationToken);
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

    private async Task<bool> ValidateUserPasswordAsync(UserId userId, RawPassword password, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId, cancellationToken);
        return user.Password.Hash == HashPassword(password, user.Password.Salt);
    }

    public async Task<AuthorizationInfo> LoginUserAsync(Email email, RawPassword password, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is null || !await ValidateUserPasswordAsync(user.Id, password, cancellationToken))
        {
            throw new IncorrectCredentialException(email);
        }
        var token = tokenGenerator.GenerateToken(user);
        return new AuthorizationInfo(user.Id, user.Name, user.Email, user.Avatar, token);
    }

    public async Task ChangePasswordAsync(UserId userId, AuthorizationVersion authorizationVersion, RawPassword newPassword, CancellationToken cancellationToken)
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

    public static PasswordHash HashPassword(RawPassword password, PasswordSalt salt)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password.Value + salt.Value));
        var strHash = Convert.ToHexStringLower(hashedBytes);
        return new PasswordHash(strHash);
    }

    public static PasswordSalt GenerateSalt()
    {
        var salt = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        var strSalt = Convert.ToHexStringLower(salt);
        return new PasswordSalt(strSalt);
    }
}

public class UserAlreadyExistsException(Email email)
    : EntityAlreadyExistsException($"User with email {email} already exists.")
{
    public readonly Email Email = email;
}

