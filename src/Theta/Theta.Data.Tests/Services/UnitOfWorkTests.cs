using FluentAssertions;
using Theta.Common.Models;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Tests.TestHelpers;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Data.Tests.Services;

public class UnitOfWorkTests : DatabaseAwareTest
{
    // Venues
    
    [Fact]
    public void Venues_ShouldInstantiateRepository_WhenCalledForTheFirstTime()
    {
        var sut = CreateSut();
        sut.Venues.Should().BeAssignableTo<IVenueRepository>();
    }
    
    [Fact]
    public void Venues_ShouldNotReinstantiateRepository_WhenCalledMoreThanOnce()
    {
        var sut = CreateSut();
        var venuesRepository = sut.Venues;

        sut.Venues.Should().Be(venuesRepository);
    }
    
    // SaveAsync

    [Fact]
    public async Task SaveAsync_ShouldCommitChangesOnContext()
    {
        var sut = CreateSut();
        _ = await sut.Venues.CreateAsync(new Venue("test venue"));

        // Hasn't committed changes from the repository
        TestDbContext.Venues.Should().BeEmpty();
        
        var result = await sut.SaveAsync();
        
        // Has committed the changes from the UoW
        result.Should().BeEquivalentTo<CommandResult>(true);
        TestDbContext.Venues.Should().HaveCount(1);
    } 
    
    // Private Methods

    private UnitOfWork CreateSut() 
        => new (TestDbContext);
}