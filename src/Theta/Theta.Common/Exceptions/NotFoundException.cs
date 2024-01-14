namespace Theta.Common.Exceptions;

/// <summary>
/// Exception used when no record is found matching user-provided criteria
/// </summary>
public class NotFoundException : ApplicationException
{
    public Type Type { get; }
    
    public Guid Id { get; }
    
    /// <summary>
    /// Initialize a new instance of the <see cref="NotFoundException"/> class
    /// </summary>
    /// <param name="type">Type of resource being requested</param>
    /// <param name="id">Unique identifier of the resource being requested</param>
    public NotFoundException(Type type, Guid id)
        : base($"{type.Name} not found with Id \"{id:D}\"")
    {
        Id = id;
        Type = type;
    }
}