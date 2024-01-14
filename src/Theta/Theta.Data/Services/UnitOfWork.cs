using Theta.Common.Models;
using Theta.Data.Context;
using Theta.Data.Repositories;
using Theta.Data.Repositories.Interfaces;

namespace Theta.Data.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly ThetaDbContext _dbContext;

    private IVenueRepository? _venueRepository;

    /// <summary>
    /// Initialize a new instance of the <see cref="UnitOfWork"/> class
    /// </summary>
    /// <param name="dbContext"></param>
    public UnitOfWork(ThetaDbContext dbContext)
        => _dbContext = dbContext;

    /// <inheritdoc />
    public IVenueRepository Venues => _venueRepository ??= new VenueRepository(_dbContext);

    /// <inheritdoc />
    public async Task<CommandResult> SaveAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}