namespace Theta.Api.Features.Venues.DTOs;

/// <summary>
/// Read model for a venue
/// </summary>
public class VenueReadDto
{
    /// <summary>
    /// Unique identifier of the venue
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the venue
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Timezone-aware timestamp of the venues creation 
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }
    
    /// <summary>
    /// Timezone-aware timestamp of the last modification to the venue
    /// </summary>
    public DateTimeOffset ModifiedDate { get; set; }
}