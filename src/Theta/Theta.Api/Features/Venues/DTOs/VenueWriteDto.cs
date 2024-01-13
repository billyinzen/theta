namespace Theta.Api.Features.Venues.DTOs;

/// <summary>
/// Data transfer object for writing to Venue endpoints
/// </summary>
/// <param name="Name">
/// The name of the venue. Must be unique to a venue, and between 8 and 100 characters in length.
/// </param>
public record VenueWriteDto(string Name);