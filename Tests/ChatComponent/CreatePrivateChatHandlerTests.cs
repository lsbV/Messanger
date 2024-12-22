using ChatComponent.ChatOperations.Create;

namespace Tests.ChatComponent;

public class CreatePrivateChatHandlerTests(SqlServerAssemblyFixture sqlFixture, MongoDbAssemblyFixture mongoDbAssemblyFixture) : IDisposable
{
    private readonly AppDbContext _context = new AppDbContext(sqlFixture.Options);
    private readonly IMongoCollection<Message> _messageCollection = mongoDbAssemblyFixture.MessageCollection;
    private bool _disposedValue;

    [Fact]
    public async Task Handle_ShouldCreatePrivateChatAndInsertMessage()
    {
        // Arrange
        var handler = new CreatePrivateChatHandler(_context, _messageCollection);
        var user1 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var user2 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var firstMessage = new TextContent("Hello!");
        var command = new CreatePrivateChatCommand(user1.Id, user2.Id, firstMessage);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(user1.Id, result.UserId1);
        Assert.Equal(user2.Id, result.UserId2);
        var chat = await _context.PrivateChats.FirstOrDefaultAsync(c => c.UserId1 == user1.Id && c.UserId2 == user2.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(chat);
        var message = await _messageCollection.Find(m => m.RecipientId == chat.Id).FirstOrDefaultAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(message);
        Assert.Equal(user1.Id, message.SenderId);
        Assert.Equal(firstMessage, message.Content);
        Assert.Equal(MessageStatus.Sent, message.Status);
    }

    [Fact]
    public async Task Handle_ShouldRollbackTransactionWhenMessageInsertFails()
    {
        // Arrange
        var user1 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var user2 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var messageCollectionMock = new Mock<IMongoCollection<Message>>();
        messageCollectionMock.Setup(m => m.InsertOneAsync(It.IsAny<Message>(), null, CancellationToken.None))
            .ThrowsAsync(new Exception("MongoDB error"));
        var handler = new CreatePrivateChatHandler(_context, messageCollectionMock.Object);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var firstMessage = new TextContent("Hello!");
        var command = new CreatePrivateChatCommand(user1.Id, user2.Id, firstMessage);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        var chat = await _context.PrivateChats.FirstOrDefaultAsync(c => c.UserId1 == user1.Id && c.UserId2 == user2.Id, TestContext.Current.CancellationToken);
        Assert.Null(chat);
    }

    [Fact]
    public async Task Handle_ShouldReturnExistedChatIfPresent()
    {
        // Arrange
        var user1 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var user2 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomPrivateChat(_context, user1.Id, user2.Id);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var handler = new CreatePrivateChatHandler(_context, _messageCollection);
        var firstMessage = new TextContent("Hello!");
        var command = new CreatePrivateChatCommand(user1.Id, user2.Id, firstMessage);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(chat.Id, result.Id);
    }


    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            _context.Dispose();
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}