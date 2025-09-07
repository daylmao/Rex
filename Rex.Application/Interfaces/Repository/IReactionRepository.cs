using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with reactions in the database.
    /// </summary>
    public interface IReactionRepository : IGenericRepository<Reaction>
    {
        /// <summary>
        /// Gets the total number of reactions for a specific post.
        /// </summary>
        /// <param name="postId">The ID of the post.</param>
        /// <param name="targetType">The type of reaction target.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The total number of reactions for the post.</returns>
        Task<int> GetReactionCountByPostIdAsync(Guid postId, ReactionTargetType targetType, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the total number of reactions for a specific comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment.</param>
        /// <param name="targetType">The type of reaction target.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The total number of reactions for the comment.</returns>
        Task<int> GetReactionCountByCommentIdAsync(Guid commentId, ReactionTargetType targetType, CancellationToken cancellationToken);
    }
}