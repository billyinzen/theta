using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Theta.Common.Helpers;
using Theta.Data.Context;
using Theta.Domain.Features.Venues;

namespace Theta.Data.Tests.Context;

public class ThetaDbContextTests
{
    // SaveChangesAsync

    [Fact]
    public async Task SaveChangesAsync_SetsValues_ForAddedEntities()
    {
        var entity = new Venue("test entity");
        
        var fixedTimestamp = DateTimeOffset.UnixEpoch;
        DateTimeOffsetHelper.Set(fixedTimestamp);
        
        var sut = CreateSut();

        await sut.AddAsync(entity);
        await sut.SaveChangesAsync();
        
        var createdEntity = await sut.Venues.SingleAsync();
        
        createdEntity.Id.Should().NotBe(Guid.Empty);
        createdEntity.CreatedDate.Should().Be(fixedTimestamp);
        createdEntity.ModifiedDate.Should().Be(fixedTimestamp);
    }
    
    [Fact]
    public async Task SaveChangesAsync_SetsValues_ForModifiedEntities()
    {
        var entity = new Venue("test entity");
        var sut = CreateSut();
        
        // Add an entity
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        await sut.AddAsync(entity);
        await sut.SaveChangesAsync();

        // Update the entity
        var fixedTimestamp = new DateTimeOffset(1990, 1, 26, 10, 38, 15, TimeSpan.FromHours(1));
        DateTimeOffsetHelper.Set(fixedTimestamp);
        entity.Name = "updated name";
        sut.Update(entity);
        await sut.SaveChangesAsync();
        
        var modifiedEntity = await sut.Venues.SingleAsync();
        modifiedEntity.CreatedDate.Should().Be(DateTimeOffset.UnixEpoch);
        modifiedEntity.ModifiedDate.Should().Be(fixedTimestamp);
    }
    
    // Private Methods
    
    private ThetaDbContext CreateSut()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var contextOptions = new DbContextOptionsBuilder<ThetaDbContext>()
            .UseSqlite(connection)
            .Options;

        var sut = new ThetaDbContext(contextOptions);
        sut.Database.EnsureCreated();
        
        return sut;
    }
}