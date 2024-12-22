namespace Core.Exceptions;

public class IncorrectCredentialException(Email email) : Exception($"Incorrect credentials for user {email}.")
{
    public Email Email { get; } = email;
}