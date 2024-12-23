using MessageComponent.MessageOperations;

namespace Tests.MessageComponent;

public class DeleteMessageHandlerTests
{
    private readonly IMongoCollection<Message> _messageCollection;
    private readonly DeleteMessageHandler _handler;

    public DeleteMessageHandlerTests(MongoFixture mongoFixture)
    {
        _messageCollection = mongoFixture.MessageCollection;
        _handler = new DeleteMessageHandler(_messageCollection);
    }

    [Fact]
    public async Task Handle_WhenMessageExist_ShouldDeleteMessage()
    {
        // Arrange
        var message = EntityFactory.CreateAndAddToContextRandomMessage();
        await _messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        var request = new DeleteMessageRequest(message.Id);
        // Act
        await _handler.Handle(request, TestContext.Current.CancellationToken);
        // Assert
        var deletedMessage = await _messageCollection.Find(m => m.Id == message.Id).FirstOrDefaultAsync(TestContext.Current.CancellationToken);
        Assert.Null(deletedMessage);
    }

    [Fact]
    public async Task Handle_WhenMessageNotExist_ShouldThrowException()
    {
        // Arrange
        var request = new DeleteMessageRequest(EntityFactory.RandomMessageId());
        // Act & Assert
        await Assert.ThrowsAsync<MessageNotFoundException>(() => _handler.Handle(request, TestContext.Current.CancellationToken));
    }
}