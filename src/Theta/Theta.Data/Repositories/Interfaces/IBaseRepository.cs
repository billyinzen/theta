using Theta.Common.Models;

namespace Theta.Data.Repositories.Interfaces;

public interface IBaseRepository<TEntity> 
    where TEntity: BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<CommandResult> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}