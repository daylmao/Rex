using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class GenericRepository<TEntity>(RexContext context): IGenericRepository<TEntity> where TEntity : AuditableEntity
{
    public async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Set<TEntity>().FindAsync(id, cancellationToken);

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        context.Set<TEntity>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
        await SaveAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await context.Set<TEntity>()
            .Where(e => e.Id == entity.Id)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(e => e.Deleted, true)
                    .SetProperty(e => e.DeletedAt, DateTime.UtcNow),
                cancellationToken);
    }

    public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await context.Set<TEntity>().AddAsync(entity, cancellationToken);
        await SaveAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);

    public async Task<bool> ValidateAsync(Expression<Func<TEntity, bool>> validation, CancellationToken cancellationToken) =>
        await context.Set<TEntity>()
            .AsNoTracking()
            .AnyAsync(validation, cancellationToken);
}