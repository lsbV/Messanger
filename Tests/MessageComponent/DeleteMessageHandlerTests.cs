using Core.Exceptions;
using MessageComponent.MessageOperations;

namespace Tests.MessageComponent;

public class DeleteMessageHandlerTests
{
    private readonly IMongoCollection<Message> _messageCollection;
    private readonly AppDbContext _context;
    private readonly DeleteMessageHandler _handler;

    public DeleteMessageHandlerTests(MongoFixture mongoFixture, SqlFixture sqlFixture)
    {
        _messageCollection = mongoFixture.MessageCollection;
        _context = new AppDbContext(sqlFixture.Options);
        _handler = new DeleteMessageHandler(_messageCollection, _context);
    }

    [Fact]
    public async Task Handle_WhenMessageExistAndUserIsAuthor_ShouldDeleteMessage()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [user]);
        var message = EntityFactory.CreateRandomMessage(chat.Id, user.Id);
        await _messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        var request = new DeleteMessageCommand(user.Id, message.Id);
        // Act
        await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        var deletedMessage = await _messageCollection.Find(m => m.Id == message.Id).FirstOrDefaultAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(deletedMessage);
        Assert.NotNull(deletedMessage.EditedAt);
        Assert.IsType<DeletedContent>(deletedMessage.Content);
    }

    [Fact]
    public async Task Handle_WhenMessageNotExist_ShouldThrowException()
    {
        // Arrange
        var request = new DeleteMessageCommand(UserId.New(), EntityFactory.RandomMessageId());
        // Act & Assert
        await Assert.ThrowsAsync<MessageNotFoundException>(() => _handler.Handle(request, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Handle_WhenUserIsNotSenderAndNotManagerOrOwner_ShouldThrowException()
    {
        // Arrange
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, []);
        var message = EntityFactory.CreateRandomMessage(chat.Id, EntityFactory.CreateRandomUserId());
        await _messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var request = new DeleteMessageCommand(UserId.New(), message.Id);
        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenOperationException>(() => _handler.Handle(request, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Handle_WhenUserIsNotSenderButGroupOwner_ShouldDeleteMessage()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [user]);
        var message = EntityFactory.CreateRandomMessage(chat.Id, user.Id);
        await _messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var request = new DeleteMessageCommand(user.Id, message.Id);
        // Act
        await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        var deletedMessage = await _messageCollection.Find(m => m.Id == message.Id).FirstOrDefaultAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(deletedMessage);
        Assert.NotNull(deletedMessage.EditedAt);
        Assert.IsType<DeletedContent>(deletedMessage.Content);
    }
}