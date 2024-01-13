using Theta.Common.Models;

namespace Theta.Domain.Features.Venues;

/// <summary>
/// Representation of a Venue
/// </summary>
public class Venue : BaseEntity
{
    public Venue()
    {
    }
    
    public Venue(string name)
    {
        Name = name;
    }
    
    public string Name { get; set; } = default!;
}