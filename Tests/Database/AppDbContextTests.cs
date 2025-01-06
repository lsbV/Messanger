namespace Tests.Database;

public class AppDbContextTests(SqlFixture sqlFixture)
{
    private readonly AppDbContext _context = new AppDbContext(sqlFixture.Options);

    [Fact]
    public async Task GetChatMembersAsync_ShouldReturnMembers()
    {
        // Arrange
        var members = Enumerable.Range(0, 3).Select(_ => EntityFactory.CreateAndAddToContextRandomUser(_context)).ToList();
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, members);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        var result = await _context.GetChatMembersAsync(chat.Id, TestContext.Current.CancellationToken);
        // Assert
        var memberIds = members.Select(u => u.Id).ToList();
        memberIds.Sort((id1, id2) => id1.Value.CompareTo(id2.Value));
        result.Sort((id1, id2) => id1.Value.CompareTo(id2.Value));
        Assert.Equal(memberIds, result);
    }

    [Fact]
    public async Task CheckUserAndChatExistenceAsync_WhenUserAndChatExist_ShouldReturnVerificationResult()
    {
        // Arrange
        var user1 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var user2 = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var chat = EntityFactory.CreateAndAddToContextRandomPrivateChat(_context, user1.Id, user2.Id);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        var result = await _context.CheckUserAndChatExistenceAsync(user1.Id, chat.Id, TestContext.Current.CancellationToken);
        // Assert
        Assert.True(result.UserExists);
        Assert.True(result.ChatExists);
        Assert.True(result.IsMemberOfChat);
    }

    [Fact]
    public async Task CheckUserAndChatExistenceAsync_WhenUserDoesNotExist_ShouldReturnVerificationResult()
    {
        // Arrange
        var user = EntityFactory.CreateRandomUserId();
        var chat = EntityFactory.CreateRandomChatId();
        // Act
        var result = await _context.CheckUserAndChatExistenceAsync(user, chat, TestContext.Current.CancellationToken);
        // Assert
        Assert.False(result.UserExists);
        Assert.False(result.ChatExists);
        Assert.False(result.IsMemberOfChat);
    }

    [Fact]
    public async Task CheckUserAndChatExistenceAsync_WhenChatDoesNotExist_ShouldReturnVerificationResult()
    {
        // Arrange
        var user = EntityFactory.CreateAndAddToContextRandomUser(_context);
        var fakeChatId = EntityFactory.CreateRandomChatId();
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        var result = await _context.CheckUserAndChatExistenceAsync(user.Id, fakeChatId, TestContext.Current.CancellationToken);
        // Assert
        Assert.True(result.UserExists);
        Assert.False(result.ChatExists);
        Assert.False(result.IsMemberOfChat);
    }

    [Fact]
    public async Task CheckUserAndChatExistenceAsync_WhenUserIsNotMemberOfChat_ShouldReturnVerificationResult()
    {
        // Arrange
        var notMemberOfChat = EntityFactory.CreateRandomUser();
        var chat = EntityFactory.CreateAndAddToContextRandomGroupChat(_context, []);
        _context.Add(notMemberOfChat);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Act
        var result = await _context.CheckUserAndChatExistenceAsync(notMemberOfChat.Id, chat.Id, TestContext.Current.CancellationToken);
        // Assert
        Assert.True(result.UserExists);
        Assert.True(result.ChatExists);
        Assert.False(result.IsMemberOfChat);
    }


}