namespace Tests.ChatComponent;

public class GetChatByIdHandlerTests(SqlFixture sqlFixture) : IDisposable
{
    private readonly AppDbContext _context = new AppDbContext(sqlFixture.Options);


    [Fact]
    public async Task Handle_ShouldReturnGroupChat()
    {
        // Arrange
        var handler = new GetChatByIdHandler(_context);
        var owner = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, [owner]);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
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
        var handler = new GetChatByIdHandler(_context);
        var user1 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var user2 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomPrivateChat(_context, user1.Id, user2.Id);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
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
        _context.Dispose();
    }

}