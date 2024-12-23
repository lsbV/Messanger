using Core.Exceptions;

namespace Tests.AuthorizationComponent;

public class AuthorizationServiceTests : IDisposable
{
    private readonly AuthorizationService _authorizationService;
    private readonly AppDbContext _context;

    public AuthorizationServiceTests(SqlFixture sqlFixture, TokenGeneratorAssemblyFixture tokenGeneratorFixture)
    {
        _context = new AppDbContext(sqlFixture.Options);

        _authorizationService = new AuthorizationService(_context, new TokenGenerator(tokenGeneratorFixture.SigningCredentials, tokenGeneratorFixture.Options));
    }

    [Fact]
    public async Task GetUserAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        var result = await _authorizationService.GetUserAsync(user.Id, TestContext.Current.CancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetUserAsync_WhenUserDoesNotExist_ThrowsUserNotFoundException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        // Act
        async Task Act() => await _authorizationService.GetUserAsync(userId, TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        var result = await _authorizationService.GetUserByEmailAsync(user.Email, TestContext.Current.CancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenUserDoesNotExist_ThrowsUserNotFoundException()
    {
        // Arrange
        var email = new Email(Guid.NewGuid().ToString());
        // Act
        async Task Act() => await _authorizationService.GetUserByEmailAsync(email, TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Fact]
    public async Task LoginUserAsync_WhenUserExistsAndPasswordIsValid_ReturnsAuthorizationInfo()
    {
        // Arrange
        var salt = AuthorizationService.GenerateSalt();
        const string password = "password";
        var hash = AuthorizationService.HashPassword(password, salt);
        var user = EntityFactory.CreateRandomUser() with { Password = new Password(hash, salt) };
        _context.Add(user);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        var result = await _authorizationService.LoginUserAsync(user.Email, password, TestContext.Current.CancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Avatar, result.Avatar);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task LoginUserAsync_WhenUserDoesNotExist_ThrowsIncorrectCredentialException()
    {
        // Arrange
        var email = new Email(Guid.NewGuid().ToString());
        // Act
        async Task Act() => await _authorizationService.LoginUserAsync(email, Guid.NewGuid().ToString(), TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<IncorrectCredentialException>(Act);
    }

    [Fact]
    public async Task LoginUserAsync_WhenUserExistsAndPasswordIsInvalid_ThrowsIncorrectCredentialException()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        async Task Act() => await _authorizationService.LoginUserAsync(user.Email, Guid.NewGuid().ToString(), TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<IncorrectCredentialException>(Act);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserExistsAndPasswordIsValid_PasswordIsChanged()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var oldPassword = user.Password;
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var newPassword = Guid.NewGuid().ToString();
        // Act
        await _authorizationService.ChangePasswordAsync(user.Id, user.AuthorizationVersion, newPassword, TestContext.Current.CancellationToken);
        // Assert
        var result = await _context.Users.FindAsync([user.Id], TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEqual(oldPassword, result.Password);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserDoesNotExist_ThrowsUserNotFoundException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        // Act
        async Task Act() => await _authorizationService.ChangePasswordAsync(userId, new AuthorizationVersion(1), Guid.NewGuid().ToString(), TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenAuthorizationVersionIsInvalid_ThrowsForbiddenOperationException()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        async Task Act() => await _authorizationService.ChangePasswordAsync(user.Id, user.AuthorizationVersion.Next(), Guid.NewGuid().ToString(), TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<ForbiddenOperationException>(Act);
    }

    [Fact]
    public async Task ValidateAuthorizationVersionAsync_WhenAuthorizationVersionIsValid_ReturnsTrue()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        var result = await _authorizationService.ValidateAuthorizationVersionAsync(user.Id, user.AuthorizationVersion, TestContext.Current.CancellationToken);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateAuthorizationVersionAsync_WhenAuthorizationVersionIsInvalid_ReturnsFalse()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        var result = await _authorizationService.ValidateAuthorizationVersionAsync(user.Id, AuthorizationVersion.Default, TestContext.Current.CancellationToken);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GenerateSalt_ReturnsSalt()
    {
        // Act
        var result = AuthorizationService.GenerateSalt();
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }


    [Fact]
    public void HashPassword_WhenPasswordIsValid_ReturnsHashedPassword()
    {
        // Arrange
        var password = "password";
        var salt = AuthorizationService.GenerateSalt();
        // Act
        var result1 = AuthorizationService.HashPassword(password, salt);
        var result2 = AuthorizationService.HashPassword(password, salt);
        // Assert
        Assert.NotNull(result1);
        Assert.NotEmpty(result1);
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void HashPassword_WhenPasswordIsInvalid_ReturnsDifferentHashedPassword()
    {
        // Arrange
        var password1 = "password1";
        var password2 = "password2";
        var salt = AuthorizationService.GenerateSalt();
        // Act
        var result1 = AuthorizationService.HashPassword(password1, salt);
        var result2 = AuthorizationService.HashPassword(password2, salt);
        // Assert
        Assert.NotNull(result1);
        Assert.NotEmpty(result1);
        Assert.NotNull(result2);
        Assert.NotEmpty(result2);
        Assert.NotEqual(result1, result2);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

}