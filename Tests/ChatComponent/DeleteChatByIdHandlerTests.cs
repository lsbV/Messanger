
using ChatComponent.Exceptions;

namespace Tests.ChatComponent;

public class DeleteChatByIdHandlerTests(SqlFixture sqlFixture, MongoFixture mongoFixture) : IDisposable
{
    private readonly AppDbContext context = new AppDbContext(sqlFixture.Options);
    private readonly IMongoCollection<Message> messageCollection = mongoFixture.MessageCollection;


    [Fact]
    public async Task Handle_ShouldDeleteGroupChatAndMessages()
    {
        // Arrange
        var handler = new DeleteChatByIdHandler(context, messageCollection);
        var owner = EntityFactory.CreateAndAddToContextRandomUser(context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(context, [owner]);
        var message = EntityFactory.CreateAndAddToContextRandomMessage(chat.Id, owner.Id);
        await messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var query = new DeleteChatByIdCommand(chat.Id);
        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        var result = await context.Chats.FindAsync([chat.Id], TestContext.Current.CancellationToken);
        Assert.Null(result);
        var messages = await messageCollection.FindAsync(m => m.RecipientId == chat.Id, null, TestContext.Current.CancellationToken);
        Assert.Empty(await messages.ToListAsync(TestContext.Current.CancellationToken));
    }
    [Fact]
    public async Task Handle_ShouldDeletePrivateChatAndMessages()
    {
        // Arrange
        var handler = new DeleteChatByIdHandler(context, messageCollection);
        var user1 = EntityFactory.CreateAndAddToContextRandomUser(context);
        var user2 = EntityFactory.CreateAndAddToContextRandomUser(context);
        var chat = EntityFactory.CreateAndAddToContextRandomPrivateChat(context, user1.Id, user2.Id);
        var message = EntityFactory.CreateAndAddToContextRandomMessage(chat.Id, user1.Id);
        await messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var query = new DeleteChatByIdCommand(chat.Id);
        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        var result = await context.Chats.FindAsync([chat.Id], TestContext.Current.CancellationToken);
        Assert.Null(result);
        var messages = await messageCollection.FindAsync(m => m.RecipientId == chat.Id, null, TestContext.Current.CancellationToken);
        Assert.Empty(await messages.ToListAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Handle_ShouldThrowChatNotFoundException()
    {
        // Arrange
        var handler = new DeleteChatByIdHandler(context, messageCollection);
        var chatId = new ChatId(Guid.NewGuid());
        var query = new DeleteChatByIdCommand(chatId);
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
        var handler = new DeleteChatByIdHandler(context, mockMessageCollection.Object);
        mockMessageCollection.Setup(m => m.DeleteManyAsync(It.IsAny<FilterDefinition<Message>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var owner = EntityFactory.CreateAndAddToContextRandomUser(context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(context, [owner]);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var query = new DeleteChatByIdCommand(chat.Id);
        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(query, CancellationToken.None));

        // Assert
        Assert.IsType<Exception>(exception);
        var result = await context.Chats.FindAsync([chat.Id], TestContext.Current.CancellationToken);
        Assert.NotNull(result);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}