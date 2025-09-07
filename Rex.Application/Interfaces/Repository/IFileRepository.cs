using Rex.Application.Pagination;
using Rex.Enum;
using File = Rex.Models.File;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with files in the database.
    /// </summary>
    public interface IFileRepository : IGenericRepository<File>
    {
        /// <summary>
        /// Gets a paginated list of files for a specific entity filtered by file type.
        /// </summary>
        /// <param name="targetId">The ID of the target entity.</param>
        /// <param name="targetType">The type of the target entity.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="type">The type of files to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of files matching the entity and file type.</returns>
        Task<PagedResult<File>> GetFilesPaginatedByEntityAndTypeAsync(Guid targetId, TargetType targetType, int page,
            int size, FileType type, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a paginated list of files for a specific entity, regardless of file type.
        /// </summary>
        /// <param name="targetId">The ID of the target entity.</param>
        /// <param name="targetType">The type of the target entity.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of files for the entity.</returns>
        Task<PagedResult<File>> GetFilesPaginatedByEntityAsync(Guid targetId, TargetType targetType, int page,
            int size, CancellationToken cancellationToken);
    }
}