namespace Theta.Domain.Features.Venues;

/// <summary>
/// Constants applicable to the <see cref="Venue"/> type
/// </summary>
public static class VenueConstants
{
    /// <summary>
    /// The minimum length of the <see cref="Venue.Name"/> property
    /// </summary>
    public const int NameMinimumLength = 8;
    
    /// <summary>
    /// The maximum length of the <see cref="Venue.Name"/> property
    /// </summary>
    public const int NameMaximumLength = 100;
}