using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with comments in the database.
    /// </summary>
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        /// <summary>
        /// Gets a paginated list of comments for a specific post.
        /// </summary>
        /// <param name="postId">The ID of the post.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of comments for the post.</returns>
        Task<PagedResult<Comment>> GetCommentsPaginatedByPostIdAsync(Guid postId, int page, int size,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets a paginated list of replies to a specific comment in a post.
        /// </summary>
        /// <param name="postId">The ID of the post.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="parentCommentId">The ID of the parent comment.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of comment replies.</returns>
        Task<PagedResult<Comment>> GetCommentsRepliedPaginatedByParentCommentIdAsync(Guid postId, int page, int size,
            Guid parentCommentId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the total number of comments for a specific post.
        /// </summary>
        /// <param name="postId">The ID of the post.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The total number of comments.</returns>
        Task<int> GetCommentsCountByPostIdAsync(Guid postId, CancellationToken cancellationToken);

        Task<Dictionary<Guid, int>> GetCommentsCountByPostIdsAsync(IEnumerable<Guid> postIds,
            CancellationToken cancellationToken);
    }
}