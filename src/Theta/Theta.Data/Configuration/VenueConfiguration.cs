using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Theta.Domain.Features.Venues;

namespace Theta.Data.Configuration;

public static class VenueConfiguration
{
    public static void Configure(EntityTypeBuilder<Venue> entity)
    {
        entity.HasQueryFilter(e => !e.IsDeleted);
        
        entity.HasIndex(e => e.Name)
            .IsUnique();

        entity.Property(e => e.Name)
            .HasMaxLength(VenueConstants.NameMaximumLength);
    }
}