using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with posts in the database.
    /// </summary>
    public interface IPostRepository : IGenericRepository<Post>
    {
        /// <summary>
        /// Gets a paginated list of posts for a specific group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of posts for the group.</returns>
        Task<PagedResult<Post>> GetPostsByGroupIdAsync(Guid groupId, int page, int size,
            CancellationToken cancellationToken);
    }
}