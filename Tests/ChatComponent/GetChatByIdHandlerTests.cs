using ChatComponent.ChatOperations.Get;

namespace Tests.ChatComponent;

public class GetChatByIdHandlerTests(SqlServerAssemblyFixture sqlFixture) : IDisposable
{
    private readonly AppDbContext context = new AppDbContext(sqlFixture.Options);


    [Fact]
    public async Task Handle_ShouldReturnGroupChat()
    {
        // Arrange
        var handler = new GetChatByIdHandler(context);
        var owner = EntityFactory.CreateAndAddToContextRandomUser(context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(context, [owner]);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var query = new GetChatByIdQuery(chat.Id);
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<GroupChat>(chat);
        var resultGroupChat = (GroupChat)result;
        Assert.Equal(chat.Id, result.Id);
        Assert.Equal(chat.ChatName, resultGroupChat.ChatName);
        Assert.Contains(resultGroupChat.Users, u => u.Id == owner.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnPrivateChat()
    {
        // Arrange
        var handler = new GetChatByIdHandler(context);
        var user1 = EntityFactory.CreateAndAddToContextRandomUser(context);
        var user2 = EntityFactory.CreateAndAddToContextRandomUser(context);
        var chat = EntityFactory.CreateAndAddToContextRandomPrivateChat(context, user1.Id, user2.Id);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var query = new GetChatByIdQuery(chat.Id);
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<PrivateChat>(chat);
        var resultPrivateChat = (PrivateChat)result;
        Assert.Equal(chat.Id, result.Id);
        Assert.Equal(chat.UserId1, resultPrivateChat.UserId1);
        Assert.Equal(chat.UserId2, resultPrivateChat.UserId2);
    }

    public void Dispose()
    {
        context.Dispose();
    }

}