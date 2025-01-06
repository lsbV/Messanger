using ChatComponent.Exceptions;

namespace Tests.ChatComponent;

public class LeaveGroupChatHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly LeaveGroupChatHandler _handler;

    public LeaveGroupChatHandlerTests(SqlFixture sqlFixture)
    {
        _context = new AppDbContext(sqlFixture.Options);
        _handler = new LeaveGroupChatHandler(_context);
    }

    [Fact]
    public async Task Handle_WhenChatUserDoesNotExist_ThrowsGroupChatUserNotFoundException()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, []);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var command = new LeaveGroupChatCommand(chat.Id, user.Id);
        // Act & Assert
        await Assert.ThrowsAsync<GroupChatUserNotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenChatUserIsOwner_ThrowsOwnerCannotLeaveGroupChatException()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [user]);
        var command = new LeaveGroupChatCommand(chat.Id, user.Id);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act & Assert
        await Assert.ThrowsAsync<OwnerCannotLeaveGroupChatException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenChatUserIsNotOwner_RemovesChatUser()
    {
        // Arrange
        var owner = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [owner, user]);
        var command = new LeaveGroupChatCommand(chat.Id, user.Id);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        await _handler.Handle(command, CancellationToken.None);
        // Assert
        var chatUser = await _context.GroupChatUsers.FindAsync([chat.Id, user.Id], TestContext.Current.CancellationToken);
        Assert.Null(chatUser);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}