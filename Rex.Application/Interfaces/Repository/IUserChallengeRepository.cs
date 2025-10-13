using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with user challenges in the database.
    /// </summary>
    public interface IUserChallengeRepository : IGenericRepository<UserChallenge>
    {
        /// <summary>
        /// Gets the total number of challenges associated with a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The total number of challenges for the user.</returns>
        Task<int> GetChallengesCountByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task<UserChallenge> GetByUserAndChallengeAsync(Guid userId, Guid challengeId,
            CancellationToken cancellationToken);
    }
}