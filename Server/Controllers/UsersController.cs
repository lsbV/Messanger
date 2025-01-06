namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IAuthorizationService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request, CancellationToken cancellationToken)
    {
        await using var avatarStream = request.Avatar.OpenReadStream();
        var registerUserInfo = new RegisterUserInfo(
            new UserName(request.UserName),
            new Email(request.Email),
            new RawPassword(request.Password),
            new AvatarReadOnlyFileStream(avatarStream));
        await service.RegisterUserAsync(registerUserInfo, cancellationToken);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authorizationInfo = await service.LoginUserAsync(new Email(request.Email), new RawPassword(request.Password), cancellationToken);
        return Ok(authorizationInfo.ToDto());
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await service.ChangePasswordAsync(
            this.GetUserId(),
            this.GetAuthorizationVersion(),
            new RawPassword(request.NewPassword),
            cancellationToken);
        return Ok();
    }
}