using Theta.Common.Models;
using Theta.Data.Repositories.Interfaces;

namespace Theta.Data.Services;

public interface IUnitOfWork
{
    /// <summary>
    /// Repository exposing data layer methods for Venues
    /// </summary>
    IVenueRepository Venues { get; }

    /// <summary>
    /// Save changes made in the current unit of work
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CommandResult> SaveAsync(CancellationToken cancellationToken = default);
}