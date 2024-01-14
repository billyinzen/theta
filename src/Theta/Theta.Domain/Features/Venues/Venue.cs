using Theta.Common.Models;

namespace Theta.Domain.Features.Venues;

/// <summary>
/// Representation of a Venue
/// </summary>
public class Venue : BaseEntity
{
    /// <summary>
    /// Initialize a new instance of the <see cref="Venue"/> class
    /// </summary>
    /// <param name="name">The name of the venue</param>
    public Venue(string name)
    {
        Name = name;
    }
    
    /// <summary>
    /// The name of the venue
    /// </summary>
    public string Name { get; set; } = default!;
}