using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with notifications in the database.
    /// </summary>
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        /// <summary>
        /// Gets a paginated list of notifications for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of notifications for the user.</returns>
        Task<PagedResult<Notification>> GetNotificationsByUserIdAsync(Guid userId, int page, int size,
            CancellationToken cancellationToken);
    }
}