namespace Theta.Common.Exceptions;

/// <summary>
/// Exception used when no record is found matching user-provided criteria
/// </summary>
public class ConflictException : ApplicationException
{
    public Type Type { get; }
    
    public Guid Id { get; }
    
    public string EntityTag { get; }
    
    public string ProvidedEntityTag { get; }

    /// <summary>
    /// Initialize a new instance of the <see cref="NotFoundException"/> class
    /// </summary>
    /// <param name="type">Type of resource being requested</param>
    /// <param name="id">Unique identifier of the resource being requested</param>
    /// <param name="entityTag">Etag of the matched resource</param>
    /// <param name="providedEntityTag">Etag provided in the request body</param>
    public ConflictException(Type type, Guid id, string entityTag, string providedEntityTag)
        : base($"{type.Name} not found with Id \"{id:D}\"")
    {
        Id = id;
        Type = type;
        EntityTag = entityTag;
        ProvidedEntityTag = providedEntityTag;
    }
}