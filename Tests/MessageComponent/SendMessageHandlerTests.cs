using MessageComponent.MessageOperations;

namespace Tests.MessageComponent;

public class SendMessageHandlerTests
{
    private readonly AppDbContext _context;
    private readonly IMongoCollection<Message> _messageCollection;
    private readonly SendMessageHandler _sendMessageHandler;
    public SendMessageHandlerTests(SqlFixture sqlFixture, MongoFixture mongoFixture)
    {
        _context = new AppDbContext(sqlFixture.Options);
        _messageCollection = mongoFixture.MessageCollection;
        _sendMessageHandler = new SendMessageHandler(_messageCollection, _context);
    }

    [Fact]
    public async Task SendMessageHandler_WhenUserAndChatExistAndUserIsMember_ShouldSaveToDbAndReturnMessage()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [user]);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var request = new SendMessageRequest(user.Id, chat.Id, new TextContent("Hello"));

        // Act
        var message = await _sendMessageHandler.HandleAsync(request, TestContext.Current.CancellationToken);

        //// Assert
        Assert.NotNull(message);
        var messageFromDb = await _messageCollection.Find(m => m.Id == message.Id).FirstOrDefaultAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(messageFromDb);
        Assert.Equal(message, messageFromDb);
    }

    [Fact]
    public async Task SendMessageHandler_WhenUserDoesNotExist_ShouldThrowException()
    {
        // Arrange

        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, []);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var request = new SendMessageRequest(EntityFactory.RandomUserId(), chat.Id, new TextContent("Hello"));
        // Act
        Func<Task> act = async () => await _sendMessageHandler.HandleAsync(request, TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<IncorrectMessageRequestException>(act);
    }

    [Fact]
    public async Task SendMessageHandler_WhenChatDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var request = new SendMessageRequest(user.Id, EntityFactory.RandomChatId(), new TextContent("Hello"));
        // Act
        Func<Task> act = async () => await _sendMessageHandler.HandleAsync(request, TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<IncorrectMessageRequestException>(act);
    }

    [Fact]
    public async Task SendMessageHandler_WhenUserIsNotMemberOfGroupChat_ShouldThrowException()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, []);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var request = new SendMessageRequest(user.Id, chat.Id, new TextContent("Hello"));
        // Act
        Func<Task> act = async () => await _sendMessageHandler.HandleAsync(request, TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<IncorrectMessageRequestException>(act);
    }

    [Fact]
    public async Task SendMessageHandler_WhenUserIsNotMemberOfPrivateChat_ShouldThrowException()
    {
        // Arrange
        var user1 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var user2 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var user3 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomPrivateChat(_context, user1.Id, user2.Id);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var request = new SendMessageRequest(user3.Id, chat.Id, new TextContent("Hello"));
        // Act
        Func<Task> act = async () => await _sendMessageHandler.HandleAsync(request, TestContext.Current.CancellationToken);
        // Assert
        await Assert.ThrowsAsync<IncorrectMessageRequestException>(act);
    }


}