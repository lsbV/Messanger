namespace Core.Exceptions;

public class IncorrectCredentialException(string message, string username) : Exception(message)
{
    public string Username { get; } = username;
}