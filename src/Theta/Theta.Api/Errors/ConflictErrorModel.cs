using Theta.Common.Exceptions;

namespace Theta.Api.Errors;

/// <summary>
/// Read model representing a version conflict
/// </summary>
public class ConflictErrorModel : BaseErrorModel
{
    private const string ErrorMessage = "Concurrency Check Failed";
    
    /// <summary>
    /// The type of resource being requested
    /// </summary>
    public string ResourceType { get; }
    
    /// <summary>
    /// The unique identifier of the resource being requested
    /// </summary>
    public Guid Id { get; }
    
    /// <summary>
    /// The requested version identifier
    /// </summary>
    public string Requested { get; }
    
    /// <summary>
    /// The current version identifier
    /// </summary>
    public string Current { get; }
    
    /// <summary>
    /// Initialize a new instance of the <see cref="ConflictErrorModel"/> class
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="id"></param>
    /// <param name="requested"></param>
    /// <param name="current"></param>
    public ConflictErrorModel(string resourceType, Guid id, string requested, string current)
        : base(ErrorMessage)
    {
        ResourceType = resourceType;
        Id = id;
        Requested = requested;
        Current = current;
    }

    /// <summary>
    /// Create a new <see cref="ConflictErrorModel"/> from a <see cref="ConflictException"/>
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static ConflictErrorModel FromException(ConflictException exception)
        => new(exception.Type.Name, exception.Id, exception.ProvidedEntityTag, exception.EntityTag);
}