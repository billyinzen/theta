using Microsoft.EntityFrameworkCore;
using Theta.Common.Services;
using Theta.Data.Configuration;
using Theta.Domain.Features;
using Theta.Domain.Features.Venues;

namespace Theta.Data.Context;

public class ThetaDbContext : DbContext
{
    public DbSet<Venue> Venues { get; init; } = default!;
    
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
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entity in ChangeTracker.Entries()
                     .Where(entityEntry => entityEntry is { Entity: BaseEntity, State: EntityState.Added })
                     .Select(entityEntry => entityEntry.Entity)
                     .Cast<BaseEntity>())
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTimeOffsetProvider.Now;
            entity.ModifiedDate = DateTimeOffsetProvider.Now;
        }
        
        foreach (var entity in ChangeTracker.Entries()
                     .Where(entityEntry => entityEntry is { Entity: BaseEntity, State: EntityState.Modified })
                     .Select(entityEntry => entityEntry.Entity)
                     .Cast<BaseEntity>())
        {
            entity.ModifiedDate = DateTimeOffsetProvider.Now;
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}