using MessageComponent.MessageOperations;

namespace Tests.MessageComponent;

public class EditMessageHandlerTests
{
    private readonly IMongoCollection<Message> _messageCollection;
    private readonly EditMessageHandler _handler;
    public EditMessageHandlerTests(MongoFixture mongoFixture)
    {
        _messageCollection = mongoFixture.MessageCollection;
        _handler = new EditMessageHandler(_messageCollection);
    }

    [Fact]
    public async Task Handle_WhenMessageExist_ShouldReturnUpdatedMessage()
    {
        var message = EntityFactory.CreateAndAddToContextRandomMessage();
        await _messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        var request = new EditMessageRequest(message.Id, new TextContent("new message"));

        var result = await _handler.Handle(request, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.NotEqual(message.Content, result.Content);
        Assert.NotNull(result.EditedAt);
    }

    [Fact]
    public async Task Handle_WhenMessageNotExist_ShouldThrowException()
    {
        var request = new EditMessageRequest(EntityFactory.RandomMessageId(), new TextContent("new message"));

        await Assert.ThrowsAsync<MessageNotFoundException>(() => _handler.Handle(request, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Handle_WhenMessageContentIsSame_ShouldNotUpdateMessage()
    {
        var message = EntityFactory.CreateAndAddToContextRandomMessage();
        await _messageCollection.InsertOneAsync(message, null, TestContext.Current.CancellationToken);
        var request = new EditMessageRequest(message.Id, message.Content);

        var result = await _handler.Handle(request, TestContext.Current.CancellationToken);

        var updatedMessage = await _messageCollection.Find(m => m.Id == message.Id).FirstOrDefaultAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(message.Content, result.Content);
        Assert.Null(result.EditedAt);
        Assert.Equal(message, updatedMessage);
    }

}