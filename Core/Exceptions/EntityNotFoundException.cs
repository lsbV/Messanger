namespace Core.Exceptions;

public class EntityNotFoundException<TIdentifier> : Exception
{
    public readonly TIdentifier Identifier;
    public readonly string EntityName;

    public EntityNotFoundException(TIdentifier identifier, string entityName) : base($"Entity {entityName} with identifier {identifier} not found.")
    {
        EntityName = entityName;
        Identifier = identifier;
    }


    public EntityNotFoundException(TIdentifier identifier, string entityName, string message)
        : base(message)
    {
        Identifier = identifier;
        EntityName = entityName;
    }

}