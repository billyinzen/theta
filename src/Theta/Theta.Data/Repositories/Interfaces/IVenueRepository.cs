using Theta.Domain.Features.Venues;

namespace Theta.Data.Repositories.Interfaces;

public interface IVenueRepository : IBaseRepository<Venue>
{ 
    Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default);
}