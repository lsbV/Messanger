using Database;

namespace AuthorizationComponent;

internal class AuthorizationService(AppDbContext context) : IAuthorizationService
{
    //public async Task<AuthorizationVersion> GetVersionAsync()
    //{
    //}
}

internal interface IAuthorizationService
{
}