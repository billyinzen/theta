using Theta.Common.Helpers;

namespace Theta.Domain.Features;

public abstract class BaseEntity
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
    public string EntityTag
    {
        get
        {
            var etag = ChecksumHelper.GetHashValue($"{Id:N}+{CreatedDate:O}");
            etag = ChecksumHelper.GetHashValue($"{etag}+{ModifiedDate}");
            return $"\"{etag}\"";
        }
    }
    
    /// <summary>
    /// Flag indicating whether the resource has been soft deleted
    /// </summary>
    public bool IsDeleted { get; private set; }

    public void Delete()
    {
        IsDeleted = true;
    }
    
    public bool CompareEtag(string value)
    {
        // Always false if either value is empty
        if (string.IsNullOrEmpty(EntityTag) || string.IsNullOrEmpty(value))
            return false;
        
        return EntityTag.Equals(value, StringComparison.OrdinalIgnoreCase);
    }
}