using Theta.Common.Models;
using Theta.Domain.Features;

namespace Theta.Data.Repositories.Abstract;

public interface IBaseRepository<TEntity> 
    where TEntity: BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<CommandResult> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<CommandResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<CommandResult> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}