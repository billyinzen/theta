using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Theta.Common.Models;
using Theta.Data.Repositories;
using Theta.Data.Tests.TestHelpers;
using Theta.Domain.Features.Venues;

namespace Theta.Data.Tests.Repositories;

public class VenueRepositoryTests : DatabaseAwareTest
{
    // GetAllAsync

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenDatabaseEmpty()
    {
        var sut = CreateSut();
        var result = await sut.GetAllAsync();
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnCollection_WhenDatabasePopulated()
    {
        var seedData = new[] { new Venue("venue one"), new Venue("venue two"), new Venue("venue three") };
        await Context.Venues.AddRangeAsync(seedData);
        await Context.SaveChangesAsync();
        
        var sut = CreateSut();
        var result = await sut.GetAllAsync();
        result.Should().BeEquivalentTo(seedData);
    }
    
    // GetByIdAsync
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdNotFound()
    {
        var guid = Guid.NewGuid();

        var sut = CreateSut();
        var result = await sut.GetByIdAsync(guid);
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnVenue_WhenIdFound()
    {
        var venue = new Venue("Venue One")
        {
            Id = Guid.NewGuid()
        };
        
        await Context.Venues.AddAsync(venue);
        await Context.SaveChangesAsync();
        
        var sut = CreateSut();
        var result = await sut.GetByIdAsync(venue.Id);
        result.Should().BeEquivalentTo(venue);
    }
    
    // CreateAsync

    [Fact]
    public async Task CreateAsync_ShouldAddVenueToDatabase()
    {
        var venue = new Venue("Venue One");

        var sut = CreateSut();
        
        var result = await sut.CreateAsync(venue);
        result.Should().BeEquivalentTo<CommandResult>(true);
        
        var entityEntry = Context.ChangeTracker.Entries().Single();
        entityEntry.State.Should().Be(EntityState.Added);
        entityEntry.Entity.As<Venue>().Name.Should().BeEquivalentTo(venue.Name);
    }
    
    // UpdateAsync
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateVenue()
    {
        var venue = new Venue("Venue One");
        await Context.Venues.AddAsync(venue);
        await Context.SaveChangesAsync();

        var sut = CreateSut();

        venue.Name = "New Name";
        
        var result = await sut.UpdateAsync(venue);
        result.Should().BeEquivalentTo<CommandResult>(true);
        
        var entityEntry = Context.ChangeTracker.Entries().Single();
        entityEntry.State.Should().Be(EntityState.Modified);
        entityEntry.Entity.As<Venue>().Name.Should().BeEquivalentTo(venue.Name);
    }
    
    // RemoveAsync

    [Fact]
    public async Task RemoveAsync_ShouldSoftDeleteVenue()
    {
        var venue = new Venue("Venue One");
        await Context.Venues.AddAsync(venue);
        await Context.SaveChangesAsync();

        var sut = CreateSut();
        
        var result = await sut.RemoveAsync(venue);
        result.Should().BeEquivalentTo<CommandResult>(true);
        
        var entityEntry = Context.ChangeTracker.Entries().Single();
        entityEntry.State.Should().Be(EntityState.Modified);
        entityEntry.Entity.As<Venue>().IsDeleted.Should().BeTrue();
    }
    
    // IsNameUniqueAsync
    
    [Fact]
    public async Task IsNameUniqueAsync_ShouldReturnTrue_WhenNameUnique_WithoutQualifyingId()
    {
        const string name = "venue name";
        var sut = CreateSut();
        
        var result = await sut.IsNameUniqueAsync(name);
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task IsNameUniqueAsync_ShouldReturnFalse_WhenNameInUse_WithoutQualifyingId()
    {
        const string name = "venue name";

        await Context.Venues.AddAsync(new Venue(name));
        await Context.SaveChangesAsync();
        
        var sut = CreateSut();
        
        var result = await sut.IsNameUniqueAsync(name);
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task IsNameUniqueAsync_ShouldReturnTrue_WhenNameInUseByReferencedVenue()
    {
        const string seedName = "venue name";

        var seedVenue = new Venue(seedName);
        
        await Context.Venues.AddAsync(seedVenue);
        await Context.SaveChangesAsync();
        
        var sut = CreateSut();
        
        var result = await sut.IsNameUniqueAsync(seedName, seedVenue.Id);
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task IsNameUniqueAsync_ShouldReturnTrue_WhenNameNotInUse()
    {
        const string seedName = "venue name";

        var seedVenue = new Venue(seedName);
        
        await Context.Venues.AddAsync(seedVenue);
        await Context.SaveChangesAsync();
        
        var sut = CreateSut();
        
        var result = await sut.IsNameUniqueAsync("new name", seedVenue.Id);
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task IsNameUniqueAsync_ShouldReturnFalse_WhenNameInUseByAnotherVenue()
    {
        const string seedName = "venue name";
        const string testName = "venue test";

        var seedVenue = new Venue(seedName);
        
        await Context.Venues.AddAsync(seedVenue);
        await Context.Venues.AddAsync(new Venue(testName));
        await Context.SaveChangesAsync();
        
        var sut = CreateSut();
        
        var result = await sut.IsNameUniqueAsync(testName, seedVenue.Id);
        result.Should().BeFalse();
    }
    
    // Private Methods

    private VenueRepository CreateSut() 
        => new (Context);
}