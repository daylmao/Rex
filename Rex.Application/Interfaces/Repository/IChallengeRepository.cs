using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with challenges in the database.
    /// </summary>
    public interface IChallengeRepository : IGenericRepository<Challenge>
    {
        /// <summary>
        /// Gets a paginated list of challenges by group ID and challenge status.
        /// </summary>
        /// <param name="groupId">The ID of the group the challenges belong to.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="status">The status of the challenge.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of challenges that match the criteria.</returns>
        Task<PagedResult<Challenge>> GetChallengesPaginatedByGroupIdAndStatusAsync(Guid groupId, int page, int size,
            ChallengeStatus status,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets a paginated list of challenges based on user participation in a group and challenge status.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="status">The user's participation status in the challenge.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of challenges with the user's participation.</returns>
        Task<PagedResult<Challenge>> GetChallengesPaginatedByUserParticipationGroupAndStatusAsync(Guid userId, int page, int size,
            Guid groupId, UserChallengeStatus status,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets a paginated list of challenges by user ID and participation status.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="status">The user's participation status.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of challenges the user is participating in.</returns>
        Task<PagedResult<Challenge>> GetChallengesPaginatedByUserIdAndStatusAsync(Guid userId, int page, int size,
            UserChallengeStatus status,
            CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a user has already joined a specific challenge.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="challengeId">The challenge ID.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the user has already joined the challenge; otherwise false.</returns>
        Task<bool> UserAlreadyJoined(Guid userId, Guid challengeId, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a challenge belongs to a specific group.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <param name="challengeId">The challenge ID.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the challenge belongs to the group; otherwise false.</returns>
        Task<bool> ChallengeBelongsToGroup(Guid groupId, Guid challengeId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all active challenges that have passed their end date.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A collection of expired challenges with active status.</returns>
        Task<IEnumerable<Challenge>> GetExpiredChallenges(CancellationToken cancellationToken);
    }
}