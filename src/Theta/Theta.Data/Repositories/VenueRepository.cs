using Microsoft.EntityFrameworkCore;
using Theta.Data.Context;
using Theta.Data.Repositories.Abstract;
using Theta.Data.Repositories.Interfaces;
using Theta.Domain.Features.Venues;

namespace Theta.Data.Repositories;

public class VenueRepository : BaseRepository<Venue>, IVenueRepository
{
    /// <summary>
    /// Initialize a new instance of the <see cref="VenueRepository"/> class
    /// </summary>
    /// <param name="context"></param>
    public VenueRepository(ThetaDbContext context) 
        : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default)
        => !await Context.Venues.AnyAsync(
            predicate: venue => venue.Name == name, 
            cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public async Task<bool> IsNameUniqueAsync(string name, Guid id, CancellationToken cancellationToken = default)
        => !await Context.Venues.AnyAsync(
            predicate: venue => venue.Name == name && venue.Id != id, 
            cancellationToken: cancellationToken);
}