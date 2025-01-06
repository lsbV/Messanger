using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Server.Extensions;

internal static class NotificationHubExtensions
{
    internal static UserId GetUserId(this HubCallerContext context)
    {
        var strId = context.UserIdentifier ?? throw new NotAuthenticatedException();
        return new UserId(Guid.Parse(strId));
    }

    internal static AuthorizationVersion GetAuthorizationVersion(this HubCallerContext context)
    {
        var version = int.Parse(context.User?.FindFirstValue(ClaimTypes.Version) ?? throw new NotAuthenticatedException());
        return new AuthorizationVersion(version);
    }
}