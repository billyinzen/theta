using FluentAssertions;
using Theta.Domain.Features.Venues;

namespace Theta.Domain.Tests.Features.Venues;

public class VenueTests
{
    // Ctor
    
    [Fact]
    public void Ctor_InitializesObjectWithGivenParameters()
    {
        const string name = "Name";
        var sut = new Venue(name);
        sut.Name.Should().BeEquivalentTo(name);
    }
}