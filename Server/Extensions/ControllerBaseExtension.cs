using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Server.Extensions;

public static class ControllerBaseExtension
{
    public static UserId GetUserId(this ControllerBase controller)
    {
        var guid = Guid.Parse(controller.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotAuthenticatedException());
        return new UserId(guid);
    }

    public static AuthorizationVersion GetAuthorizationVersion(this ControllerBase controller)
    {
        var version = int.Parse(controller.User.FindFirstValue(ClaimTypes.Version) ?? throw new NotAuthenticatedException());
        return new AuthorizationVersion(version);
    }



}

public class NotAuthenticatedException : Exception
{
}