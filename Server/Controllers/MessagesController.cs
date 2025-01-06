using MessageComponent.MessageOperations;

namespace Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MessagesController(ISender sender)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SendMessageRequest request)
    {
        var sendMessageCommand = new SendMessageCommand(
            this.GetUserId(),
            new ChatId(request.ChatId),
            request.Content.ToDomain());
        var message = await sender.Send(sendMessageCommand);
        return Created(string.Empty, message);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] EditMessageRequest command)
    {
        var editMessageCommand = new EditMessageCommand(
            this.GetUserId(),
            new MessageId(command.MessageId),
            command.Content.ToDomain());
        var message = await sender.Send(editMessageCommand);
        return Ok(message);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] Guid messageId)
    {
        var deleteMessageCommand = new DeleteMessageCommand(
            this.GetUserId(),
            new MessageId(messageId));
        await sender.Send(deleteMessageCommand);
        return NoContent();
    }
}