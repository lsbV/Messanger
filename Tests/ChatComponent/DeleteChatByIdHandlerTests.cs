
using ChatComponent.Exceptions;

namespace Tests.ChatComponent;

public class DeleteChatByIdHandlerTests(SqlFixture sqlFixture, MongoFixture mongoFixture) : IDisposable
{
    private readonly AppDbContext _context = new AppDbContext(sqlFixture.Options);
    private readonly IMongoCollection<Message> _messageCollection = mongoFixture.MessageCollection;


    [Fact]
    public async Task Handle_ShouldDeleteGroupChatAndMessages()
    {
        // Arrange
        var handler = new DeleteChatByIdHandler(_context, _messageCollection);
        var owner = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [owner]);
        var message = EntityFactory.CreateRandomMessage(chat.Id, owner.Id);
        await _messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var query = new DeleteChatCommand(chat.Id);
        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        var result = await _context.Chats.FindAsync([chat.Id], TestContext.Current.CancellationToken);
        Assert.Null(result);
        var messages = await _messageCollection.FindAsync(m => m.RecipientId == chat.Id, null, TestContext.Current.CancellationToken);
        Assert.Empty(await messages.ToListAsync(TestContext.Current.CancellationToken));
    }
    [Fact]
    public async Task Handle_ShouldDeletePrivateChatAndMessages()
    {
        // Arrange
        var handler = new DeleteChatByIdHandler(_context, _messageCollection);
        var user1 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var user2 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomPrivateChat(_context, user1.Id, user2.Id);
        var message = EntityFactory.CreateRandomMessage(chat.Id, user1.Id);
        await _messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var query = new DeleteChatCommand(chat.Id);
        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        var result = await _context.Chats.FindAsync([chat.Id], TestContext.Current.CancellationToken);
        Assert.Null(result);
        var messages = await _messageCollection.FindAsync(m => m.RecipientId == chat.Id, null, TestContext.Current.CancellationToken);
        Assert.Empty(await messages.ToListAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Handle_ShouldThrowChatNotFoundException()
    {
        // Arrange
        var handler = new DeleteChatByIdHandler(_context, _messageCollection);
        var chatId = new ChatId(Guid.NewGuid());
        var query = new DeleteChatCommand(chatId);
        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(query, TestContext.Current.CancellationToken));
        // Assert
        Assert.IsType<ChatNotFoundException>(exception);
        Assert.Equal(chatId, ((ChatNotFoundException)exception).Identifier);
    }

    [Fact]
    public async Task Handle_ShouldRollbackTransaction()
    {
        // Arrange
        var mockMessageCollection = new Mock<IMongoCollection<Message>>();
        var handler = new DeleteChatByIdHandler(_context, mockMessageCollection.Object);
        mockMessageCollection.Setup(m => m.DeleteManyAsync(It.IsAny<FilterDefinition<Message>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var owner = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [owner]);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var query = new DeleteChatCommand(chat.Id);
        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(query, CancellationToken.None));

        // Assert
        Assert.IsType<Exception>(exception);
        var result = await _context.Chats.FindAsync([chat.Id], TestContext.Current.CancellationToken);
        Assert.NotNull(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}