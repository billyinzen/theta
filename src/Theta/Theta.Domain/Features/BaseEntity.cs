using Theta.Common.Helpers;

namespace Theta.Common.Models;

public class BaseEntity
{
    /// <summary>
    /// Unique identifier for the resource 
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The timezone-aware timestamp of the resources creation
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }
    
    /// <summary>
    /// The timezone-aware timestamp of the last update to the resource
    /// </summary>
    public DateTimeOffset ModifiedDate { get; set; }

    /// <summary>
    /// Unique identifier for a specific version of a resource used in concurrency checks
    /// </summary>
    public string EntityTag =>
        ChecksumHelper.GetHashValue(ChecksumHelper.GetHashValue($"{Id:N}+{CreatedDate:O}") + $"+{ModifiedDate:O}");
    
    /// <summary>
    /// Flag indicating whether the resource has been soft deleted
    /// </summary>
    public bool IsDeleted { get; set; }
}