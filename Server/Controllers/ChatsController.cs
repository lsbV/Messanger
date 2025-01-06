namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatsController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new VerifiedGetChatByIdQuery(this.GetUserId(), new ChatId(id));
        var chat = await sender.Send(query);
        return Ok(chat.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateChatRequest request)
    {
        var command = new VerifiedUpdateGroupChatCommand(this.GetUserId(),
            new ChatId(id),
            new ChatName(request.Name),
            new ChatDescription(request.Description),
            new ChatImage(request.ImageUrl));
        await sender.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new VerifiedDeleteChatCommand(this.GetUserId(), new ChatId(id));
        await sender.Send(command);
        return NoContent();
    }

    [HttpPost("group")]
    public async Task<IActionResult> Create([FromBody] CreateGroupChatRequest request)
    {
        var command = new CreateGroupChatCommand(this.GetUserId(),
            new ChatName(request.Name),
            new ChatDescription(request.Description),
            new ChatImage(request.ImageUrl),
            Enum.Parse<GroupChatJoinMode>(request.JoinMode, true),
            request.Members.Select(x => new UserId(x)).ToImmutableList());

        var chat = await sender.Send(command);
        return CreatedAtAction(nameof(Get), new { id = chat.Id }, chat.ToDto());
    }

    [HttpPost("private")]
    public async Task<IActionResult> CreatePrivateChat([FromBody] CreatePrivateChatRequest request)
    {
        var command = new CreatePrivateChatCommand(this.GetUserId(), new UserId(request.UserId), request.Message.ToDomain());
        var chat = await sender.Send(command);
        return CreatedAtAction(nameof(Get), new { id = chat.Id }, chat.ToDto());
    }
}