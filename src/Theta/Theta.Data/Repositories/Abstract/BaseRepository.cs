using Microsoft.EntityFrameworkCore;
using Theta.Common.Models;
using Theta.Data.Context;

namespace Theta.Data.Repositories.Abstract;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity: BaseEntity
{
    protected readonly ThetaDbContext Context;

    /// <summary>
    /// Create a new instance of the <see cref="BaseRepository"/> class
    /// </summary>
    /// <param name="context"></param>
    protected BaseRepository(ThetaDbContext context)
    {
        Context = context;
    }
    
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>().ToListAsync(cancellationToken);

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>()
            .SingleOrDefaultAsync(entity => entity.Id.Equals(id), cancellationToken: cancellationToken);

    public async Task<CommandResult> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
        return true;
    }

    public async Task<CommandResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Update(entity);
        return await Task.FromResult<CommandResult>(true);
    }

    public async Task<CommandResult> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.Delete();
        Context.Set<TEntity>().Update(entity);
        return await Task.FromResult<CommandResult>(true);
    }
}