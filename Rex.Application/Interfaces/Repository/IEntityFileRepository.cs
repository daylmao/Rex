using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Models;
using File = Rex.Models.File;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with entity files in the database.
    /// </summary>
    public interface IEntityFileRepository : IGenericRepository<EntityFile>
    {
        /// <summary>
        /// Checks if a file exists for a specific target entity.
        /// </summary>
        /// <param name="targetId">The ID of the target entity.</param>
        /// <param name="fileId">The ID of the file.</param>
        /// <param name="targetType">The type of the target entity.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the file exists for the target entity; otherwise false.</returns>
        Task<bool> ExistsAsync(Guid targetId, Guid fileId, TargetType targetType, CancellationToken cancellationToken);

        /// <summary>
        /// Counts the number of files associated with a specific target entity.
        /// </summary>
        /// <param name="targetId">The ID of the target entity.</param>
        /// <param name="targetType">The type of the target entity.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The total number of files for the target entity.</returns>
        Task<int> CountByTargetIdAsync(Guid targetId, TargetType targetType, CancellationToken cancellationToken);
    }
}