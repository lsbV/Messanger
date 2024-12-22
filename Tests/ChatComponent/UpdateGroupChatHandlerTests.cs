using ChatComponent.ChatOperations.Update;

namespace Tests.ChatComponent;

public class UpdateGroupChatHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UpdateGroupChatHandler _handler;

    public UpdateGroupChatHandlerTests(SqlServerAssemblyFixture sqlServerFixture, MongoDbAssemblyFixture mongoDbFixture)
    {
        _context = new AppDbContext(sqlServerFixture.Options);
        _handler = new UpdateGroupChatHandler(_context);
    }

    [Fact]
    public async Task UpdateGroupChatHandler_WhenGroupChatExists_ShouldUpdateGroupChat()
    {
        // Arrange
        var groupChat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, []);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var command = new UpdateGroupChatCommand(groupChat.Id, new ChatName("New Name"), new ChatDescription("New Description"), new ChatImage("New Image"));
        // Act
        var updatedGroupChat = await _handler.Handle(command, CancellationToken.None);
        // Assert
        var events = await _context.ChatEvents.Where(e => e.ChatId == groupChat.Id).ToListAsync(TestContext.Current.CancellationToken);
        var chatFromDb = await _context.GroupChats.FirstOrDefaultAsync(g => g.Id == groupChat.Id, TestContext.Current.CancellationToken);
        Assert.Equal(chatFromDb, updatedGroupChat);
        Assert.Equal(command.ChatName, updatedGroupChat.ChatName);
        Assert.Equal(command.ChatDescription, updatedGroupChat.ChatDescription);
        Assert.Equal(command.ChatImage, updatedGroupChat.ChatImage);
        // 3 events should be created
        Assert.Equal(3, events.Count);
        Assert.Contains(events, e => e is GroupChatNameUpdatedEvent);
        Assert.Contains(events, e => e is GroupChatDescriptionUpdatedEvent);
        Assert.Contains(events, e => e is GroupChatImageUpdatedEvent);
    }

    [Fact]
    public async Task UpdateGroupChatHandler_WhenGroupChatDoesNotExist_ShouldThrowGroupChatNotFoundException()
    {
        // Arrange
        var command = new UpdateGroupChatCommand(new ChatId(Guid.NewGuid()), new ChatName("New Name"), new ChatDescription("New Description"), new ChatImage("New Image"));
        // Act
        var exception = await Record.ExceptionAsync(() => _handler.Handle(command, CancellationToken.None));
        // Assert
        Assert.IsType<GroupChatNotFoundException>(exception);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}