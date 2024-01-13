namespace Theta.Api.Features.Venues.DTOs;

public class VenueReadDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; }
    
    public DateTimeOffset ModifiedDate { get; set; }
}