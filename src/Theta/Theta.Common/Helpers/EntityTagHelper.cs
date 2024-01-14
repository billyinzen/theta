using Theta.Common.Models;

namespace Theta.Common.Helpers;

public static class EntityTagHelper
{
    public static bool CompareEtag(this BaseEntity entity, string value)
    {
        // Always false if either value is empty
        if (string.IsNullOrEmpty(entity.EntityTag) || string.IsNullOrEmpty(value))
            return false;
        
        return entity.EntityTag.Equals(value, StringComparison.OrdinalIgnoreCase);
    }

    public static string GenerateEntityTag(this BaseEntity entity)
    {
        var etag = ChecksumHelper.GetHashValue($"{entity.Id:N}+{entity.CreatedDate:O}");
        etag = ChecksumHelper.GetHashValue($"{etag}+{entity.ModifiedDate}");
        return $"\"{etag}\"";
    }
}