using System.Collections.Immutable;
using MediatR;
using Server.Behaviors;
using Server.SignalRHubs;

namespace Tests.Server;

public class ChatNotificationBehaviorTests
{
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly ChatNotificationBehavior _behaviorTests;

    public ChatNotificationBehaviorTests()
    {
        _contextMock = new Mock<AppDbContext>();
        _notificationServiceMock = new Mock<INotificationService>();
        _behaviorTests = new ChatNotificationBehavior(_notificationServiceMock.Object, _contextMock.Object);
    }

    [Fact]
    public async Task CreateGroupChatCommand_ShouldNotifyAboutChatEvent()
    {
        // Arrange
        var creator = EntityFactory.CreateRandomUserId();
        var members = Enumerable.Range(0, 3).Select(_ => EntityFactory.CreateRandomUserId()).ToList();
        var chat = EntityFactory.CreateRandomGroupChat();
        _notificationServiceMock
            .Setup(x => x.NotifyAboutChatEvent(It.IsAny<UserCreatedGroupChatEvent>(), It.IsAny<IEnumerable<UserId>>()))
            .Returns(Task.CompletedTask);
        _contextMock.Setup(c => c.Add(It.IsAny<ChatEvent>()));
        var next = new RequestHandlerDelegate<GroupChat>(() => Task.FromResult(chat));
        var command = new CreateGroupChatCommand(creator,
            It.IsAny<ChatName>(),
            It.IsAny<ChatDescription>(),
            It.IsAny<ChatImage>(),
            It.IsAny<GroupChatJoinMode>(),
            members.ToImmutableList());
        // Act
        await _behaviorTests.Handle(command, next, TestContext.Current.CancellationToken);
        // Assert
        _notificationServiceMock
            .Verify(x => x.NotifyAboutChatEvent(It.IsAny<UserCreatedGroupChatEvent>(), members), Times.Once);
    }

    [Fact]
    public async Task CreatePrivateChatCommand_ShouldNotifyAboutChatEvent()
    {
        // Arrange
        var creator = EntityFactory.CreateRandomUserId();
        var with = EntityFactory.CreateRandomUserId();
        var chat = EntityFactory.CreateRandomPrivateChat();
        _notificationServiceMock
            .Setup(x => x.NotifyAboutChatEvent(It.IsAny<UserCreatedPrivateChatEvent>(), It.IsAny<IEnumerable<UserId>>()))
            .Returns(Task.CompletedTask);
        _contextMock.Setup(c => c.Add(It.IsAny<ChatEvent>()));
        var next = new RequestHandlerDelegate<PrivateChat>(() => Task.FromResult(chat));
        var command = new CreatePrivateChatCommand(creator, with, new TextContent("text"));
        // Act
        await _behaviorTests.Handle(command, next, TestContext.Current.CancellationToken);
        // Assert
        _notificationServiceMock
            .Verify(x => x.NotifyAboutChatEvent(It.IsAny<UserCreatedPrivateChatEvent>(), new[] { creator, with }), Times.Once);
    }
}