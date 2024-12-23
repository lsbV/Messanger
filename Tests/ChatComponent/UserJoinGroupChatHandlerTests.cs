using ChatComponent.ChatOperations;
using ChatComponent.Exceptions;
using Core.Exceptions;

namespace Tests.ChatComponent;

public class UserJoinGroupChatHandlerTests
    : IDisposable
{
    private readonly UserJoinGroupChatHandler _handler;
    private readonly AppDbContext _context;

    public UserJoinGroupChatHandlerTests(SqlFixture fixture)
    {
        _context = new AppDbContext(fixture.Options);
        _handler = new UserJoinGroupChatHandler(_context);
    }

    [Fact]
    public async Task HandleAsync_WhenUserJoinsGroupChat_ShouldAddUserToGroupChat()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var groupChat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, []);
        var request = new UserJoinGroupChatRequest(user.Id, groupChat.Id);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        await _handler.Handle(request, TestContext.Current.CancellationToken);

        // Assert
        var groupChatUser = await _context.GroupChatUsers.FindAsync([groupChat.Id, user.Id], TestContext.Current.CancellationToken);
        Assert.NotNull(groupChatUser);

        var joinEvent = await _context.ChatEvents.FirstOrDefaultAsync(e => e.ChatId == groupChat.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(joinEvent);
    }

    [Fact]
    public async Task HandleAsync_WhenUserJoinsGroupChatAndAlreadyInGroupChat_ShouldThrowUserAlreadyInGroupChatException()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var groupChat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [user]);
        var request = new UserJoinGroupChatRequest(user.Id, groupChat.Id);
        var groupChatUser = new GroupChatUser(groupChat.Id, user.Id, GroupChatRole.User, DateTime.UtcNow);
        // Act
        async Task Act() => await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<UserAlreadyInGroupChatException>(Act);
    }

    [Fact]
    public async Task HandleAsync_WhenUserJoinsGroupChatAndGroupChatDoesNotExist_ShouldThrowChatNotFoundException()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var groupChat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [user]);
        var request = new UserJoinGroupChatRequest(user.Id, groupChat.Id);
        _context.Remove(groupChat);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        async Task Act() => await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<ChatNotFoundException>(Act);
    }

    [Fact]
    public async Task HandleAsync_WhenUserJoinsGroupChatAndGroupChatJoinModeIsNotFree_ShouldThrowForbiddenOperationException()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var groupChat = EntityFactory.CreateRandomGroupChat() with { JoinMode = GroupChatJoinMode.Invitation };
        var request = new UserJoinGroupChatRequest(user.Id, groupChat.Id);
        _context.Add(groupChat);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        async Task Act() => await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<ForbiddenOperationException>(Act);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}