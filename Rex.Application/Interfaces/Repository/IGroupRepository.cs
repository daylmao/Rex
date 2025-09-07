using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with groups in the database.
    /// </summary>
    public interface IGroupRepository : IGenericRepository<Group>
    {
        /// <summary>
        /// Gets a paginated list of groups associated with a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of groups for the user.</returns>
        Task<PagedResult<Group>> GetGroupsByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a group exists by its ID.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the group exists; otherwise false.</returns>
        Task<bool> GroupExistAsync(Guid groupId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the total number of groups associated with a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The total number of groups for the user.</returns>
        Task<int> GetGroupCountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}