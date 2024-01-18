using Theta.Common.Exceptions;

namespace Theta.Api.Errors;

/// <summary>
/// Read model representing NotFound errors  
/// </summary>
public class NotFoundErrorModel
    : BaseErrorModel
{
    internal const string ErrorMessage = "Resource Not Found";
    
    /// <summary>
    /// The type of resource being requested
    /// </summary>
    public string ResourceType { get; }
    
    /// <summary>
    /// The unique identifier of the resource being requested
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initialize a new instance of the <see cref="NotFoundException"/> class
    /// </summary>
    private NotFoundErrorModel(string resourceType, Guid id)
        : base(ErrorMessage)
    {
        ResourceType = resourceType;
        Id = id;
    }
    
    /// <summary>
    /// Create a new <see cref="NotFoundErrorModel"/> from a <see cref="NotFoundException"/>
    /// </summary>
    /// <param name="exception">The exception from which to generate the error model</param>
    public static NotFoundErrorModel FromException(NotFoundException exception)
        => new(exception.Type.Name, exception.Id);
}