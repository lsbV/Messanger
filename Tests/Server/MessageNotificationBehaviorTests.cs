using MediatR;
using MessageComponent.MessageOperations;
using Server.Behaviors;
using Server.SignalRHubs;

namespace Tests.Server;

public class MessageNotificationBehaviorTests
{
    private readonly Mock<AppDbContext> _contextMock;

    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly MessagesNotificationBehavior _behaviorTests;

    public MessageNotificationBehaviorTests()
    {
        _contextMock = new Mock<AppDbContext>();

        _notificationServiceMock = new Mock<INotificationService>();
        _behaviorTests = new MessagesNotificationBehavior(_notificationServiceMock.Object, _contextMock.Object);
    }


    [Fact]
    public async Task SendMessageCommand_ShouldSendMessageToMembers()
    {
        // Arrange
        var members = Enumerable.Range(0, 3).Select(_ => EntityFactory.CreateRandomUserId()).ToList();
        var message = EntityFactory.CreateRandomMessage(EntityFactory.CreateRandomChatId(), It.IsAny<UserId>());
        _notificationServiceMock
            .Setup(x => x.SendMessage(message, It.IsAny<IEnumerable<UserId>>()))
            .Returns(Task.CompletedTask);
        _contextMock.Setup(x => x.GetChatMembersAsync(message.RecipientId, TestContext.Current.CancellationToken))
            .ReturnsAsync(members);
        var next = new RequestHandlerDelegate<Message>(() => Task.FromResult(message));
        var command = new SendMessageCommand(message.SenderId, message.RecipientId, message.Content);
        // Act
        await _behaviorTests.Handle(command, next, TestContext.Current.CancellationToken);
        // Assert
        _notificationServiceMock
            .Verify(x => x.SendMessage(message, members), Times.Once);
    }

    [Fact]
    public async Task EditMessageCommand_ShouldNotifyAboutUpdateMessage()
    {
        // Arrange
        var members = Enumerable.Range(0, 3).Select(_ => EntityFactory.CreateRandomUserId()).ToList();
        var message = EntityFactory.CreateRandomMessage(EntityFactory.CreateRandomChatId(), It.IsAny<UserId>());
        _notificationServiceMock
            .Setup(x => x.NotifyAboutUpdateMessage(message, It.IsAny<IEnumerable<UserId>>()))
            .Returns(Task.CompletedTask);
        _contextMock.Setup(x => x.GetChatMembersAsync(message.RecipientId, TestContext.Current.CancellationToken))
            .ReturnsAsync(members);
        var next = new RequestHandlerDelegate<Message>(() => Task.FromResult(message));
        var command = new EditMessageCommand(message.SenderId, message.Id, message.Content);
        // Act
        await _behaviorTests.Handle(command, next, TestContext.Current.CancellationToken);
        // Assert
        _notificationServiceMock
            .Verify(x => x.NotifyAboutUpdateMessage(message, members), Times.Once);
    }

    [Fact]
    public async Task DeleteMessageCommand_ShouldNotifyAboutDeleteMessage()
    {
        // Arrange
        var members = Enumerable.Range(0, 3).Select(_ => EntityFactory.CreateRandomUserId()).ToList();
        var message = EntityFactory.CreateRandomMessage(EntityFactory.CreateRandomChatId(), It.IsAny<UserId>());
        _notificationServiceMock
            .Setup(x => x.NotifyAboutDeleteMessage(message, It.IsAny<IEnumerable<UserId>>()))
            .Returns(Task.CompletedTask);
        _contextMock.Setup(x => x.GetChatMembersAsync(message.RecipientId, TestContext.Current.CancellationToken))
            .ReturnsAsync(members);
        var next = new RequestHandlerDelegate<Message>(() => Task.FromResult(message));
        var command = new DeleteMessageCommand(message.SenderId, message.Id);
        // Act
        await _behaviorTests.Handle(command, next, TestContext.Current.CancellationToken);
        // Assert
        _notificationServiceMock
            .Verify(x => x.NotifyAboutDeleteMessage(message, members), Times.Once);
    }

}