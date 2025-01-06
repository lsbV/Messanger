namespace AuthorizationComponent;

public interface IAuthorizationService
{
    Task RegisterUserAsync(RegisterUserInfo registerUserInfo, CancellationToken cancellationToken);
    Task<User> GetUserAsync(UserId userId, CancellationToken cancellationToken);
    Task<User> GetUserByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<AuthorizationInfo> LoginUserAsync(Email email, RawPassword password, CancellationToken cancellationToken);
    Task ChangePasswordAsync(UserId userId, AuthorizationVersion authorizationVersion, RawPassword newPassword, CancellationToken cancellationToken);
    Task<bool> ValidateAuthorizationVersionAsync(UserId userId, AuthorizationVersion authorizationVersion, CancellationToken cancellationToken);
}