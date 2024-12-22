namespace AuthorizationComponent;

public class UserNotFoundException(UserId userId) : EntityNotFoundException<UserId>(userId, nameof(User));