using Theta.Data.Repositories.Abstract;
using Theta.Domain.Features.Venues;

namespace Theta.Data.Repositories.Interfaces;

public interface IVenueRepository : IBaseRepository<Venue>
{ 
    Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default);
    
    Task<bool> IsNameUniqueAsync(string name, Guid id, CancellationToken cancellationToken = default);
}