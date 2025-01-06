using Microsoft.AspNetCore.Http;

namespace Server.Models.Request;

public record RegisterRequest(string UserName, string Email, string Password, IFormFile Avatar);