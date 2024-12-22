namespace Core.Exceptions;

public class ForbiddenOperationException : Exception
{
    public readonly string Operation;
    public readonly UserId RequesterId;

    public ForbiddenOperationException(string operation, UserId requesterId) : base($"Operation {operation} is forbidden for user with id {requesterId.Value}")
    {
        Operation = operation;
        RequesterId = requesterId;
    }

    public ForbiddenOperationException(string operation, UserId requesterId, string message) : base(message)
    {
        Operation = operation;
        RequesterId = requesterId;
    }
}