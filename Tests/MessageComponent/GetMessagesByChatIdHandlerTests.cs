using MessageComponent.MessageOperations;

namespace Tests.MessageComponent;

public class GetMessagesByChatIdHandlerTests
{
    private readonly IMongoCollection<Message> _messageCollection;
    private readonly AppDbContext _context;
    private readonly GetMessagesByChatIdHandler _handler;

    public GetMessagesByChatIdHandlerTests(MongoFixture mongoFixture, SqlFixture sqlFixture)
    {
        _messageCollection = mongoFixture.MessageCollection;
        _context = new AppDbContext(sqlFixture.Options);
        _handler = new GetMessagesByChatIdHandler(_messageCollection);
    }

    [Fact]
    public async Task Handle_WhenMessagesExist_ShouldReturnMessages()
    {
        // Arrange
        var chatId = EntityFactory.CreateRandomChatId();
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var messages = Enumerable.Range(0, 5).Select(_ => EntityFactory.CreateRandomMessage(chatId, user.Id)).ToList();
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        await _messageCollection.InsertManyAsync(messages, null, TestContext.Current.CancellationToken);
        var request = new GetMessagesByChatIdRequest(chatId, user.Id, DateTime.MinValue, 5);
        // Act
        var result = await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(messages.Count, result.Count);
        messages.Sort(((m1, m2) => m2.CreatedAt.CompareTo(m1.CreatedAt)));
        Assert.Equal(messages, result);
    }

    [Fact]
    public async Task Handle_WhenMessagesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var chatId = EntityFactory.CreateRandomChatId();
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var request = new GetMessagesByChatIdRequest(chatId, user.Id, DateTime.MinValue, 5);
        // Act
        var result = await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WhenMessagesExistAndAfterIsSet_ShouldReturnMessagesAfter()
    {
        // Arrange
        var chatId = EntityFactory.CreateRandomChatId();
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var messages = Enumerable.Range(0, 5).Select(_ => EntityFactory.CreateRandomMessage(chatId, user.Id)).ToList();
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        await _messageCollection.InsertManyAsync(messages, null, TestContext.Current.CancellationToken);
        var request = new GetMessagesByChatIdRequest(chatId, user.Id, messages[2].CreatedAt, 5);
        // Act
        var result = await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(messages.Count - 3, result.Count);
        messages.Sort(((m1, m2) => m2.CreatedAt.CompareTo(m1.CreatedAt)));
        Assert.Equal(messages[0..2], result);
    }

    [Fact]
    public async Task Handle_WhenMessagesExistAndCountIsLessThanMessages_ShouldReturnMessagesCount()
    {
        // Arrange
        var chatId = EntityFactory.CreateRandomChatId();
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        const int expectedCount = 3;
        var messages = Enumerable.Range(0, 5).Select(_ => EntityFactory.CreateRandomMessage(chatId, user.Id)).ToList();
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        await _messageCollection.InsertManyAsync(messages, null, TestContext.Current.CancellationToken);
        var request = new GetMessagesByChatIdRequest(chatId, user.Id, DateTime.MinValue, expectedCount);
        // Act
        var result = await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCount, result.Count);
        messages.Sort(((m1, m2) => m2.CreatedAt.CompareTo(m1.CreatedAt)));
        Assert.Equal(messages[0..expectedCount], result);
    }

    [Fact]
    public async Task Handle_WhenMessagesExistAndCountIsMoreThanMessages_ShouldReturnAllMessages()
    {
        // Arrange
        var chatId = EntityFactory.CreateRandomChatId();
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        const int expectedCount = 10;
        var messages = Enumerable.Range(0, 5).Select(_ => EntityFactory.CreateRandomMessage(chatId, user.Id)).ToList();
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        await _messageCollection.InsertManyAsync(messages, null, TestContext.Current.CancellationToken);
        var request = new GetMessagesByChatIdRequest(chatId, user.Id, DateTime.MinValue, expectedCount);
        // Act
        var result = await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(messages.Count, result.Count);
        messages.Sort(((m1, m2) => m2.CreatedAt.CompareTo(m1.CreatedAt)));
        Assert.Equal(messages, result);
    }

}