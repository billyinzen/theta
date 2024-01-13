using Microsoft.EntityFrameworkCore;
using Theta.Common.Helpers;
using Theta.Common.Models;
using Theta.Data.Configuration;
using Theta.Domain.Features.Venues;

namespace Theta.Data.Context;

public class ThetaDbContext : DbContext
{
    public DbSet<Venue> Venues { get; set; }
    
    /// <summary>
    /// Initialize a new instance of the <see cref="ThetaDbContext"/> class
    /// </summary>
    /// <param name="options"></param>
    public ThetaDbContext(DbContextOptions<ThetaDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>(VenueConfiguration.Configure);
        base.OnModelCreating(modelBuilder);
    }

    // Override SaveChangesAsync

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entity in ChangeTracker.Entries()
                     .Where(entityEntry => entityEntry is { Entity: BaseEntity, State: EntityState.Added })
                     .Select(entityEntry => entityEntry.Entity)
                     .Cast<BaseEntity>())
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTimeOffsetHelper.Now();
            entity.ModifiedDate = DateTimeOffsetHelper.Now();
        }
        
        foreach (var entity in ChangeTracker.Entries()
                     .Where(entityEntry => entityEntry is { Entity: BaseEntity, State: EntityState.Modified })
                     .Select(entityEntry => entityEntry.Entity)
                     .Cast<BaseEntity>())
        {
            entity.ModifiedDate = DateTimeOffsetHelper.Now();
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}