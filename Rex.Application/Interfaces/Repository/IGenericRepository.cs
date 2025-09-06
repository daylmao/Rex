using System.Linq.Expressions;

namespace Rex.Application.Interfaces.Repository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken);
    
    Task SaveAsync(CancellationToken cancellationToken);
    
    Task<bool> ValidateAsync(Expression<Func<TEntity, bool>> validation, CancellationToken cancellationToken);
}