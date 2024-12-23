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
        var command = new VerifiedUpdateGroupChatCommand(this.GetUserId(), new ChatId(id), new ChatName(request.Name), new ChatDescription(request.Description), new ChatImage(request.ImageUrl));
        await sender.Send(command);
        return NoContent();
    }

}

public class UpdateChatRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }

}