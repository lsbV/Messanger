namespace AuthorizationComponent;

internal interface IAuthorizationService
{
    Task RegisterUserAsync(User user);
    Task<User> GetUserAsync(UserId userId, CancellationToken cancellationToken);
    Task<User> GetUserByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<AuthorizationInfo> LoginUserAsync(Email email, string password, CancellationToken cancellationToken);
    Task ChangePasswordAsync(UserId userId, AuthorizationVersion authorizationVersion, string newPassword, CancellationToken cancellationToken);
    Task<bool> ValidateAuthorizationVersionAsync(UserId userId, AuthorizationVersion authorizationVersion, CancellationToken cancellationToken);
}