using System.Linq.Expressions;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines generic CRUD operations for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The entity with the specified ID.</returns>
        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new entity in the database.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Saves changes made in the context to the database.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task SaveAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Validates an entity based on a specified condition.
        /// </summary>
        /// <param name="validation">The expression representing the validation condition.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the entity satisfies the condition; otherwise false.</returns>
        Task<bool> ValidateAsync(Expression<Func<TEntity, bool>> validation, CancellationToken cancellationToken);
        
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    }
}
