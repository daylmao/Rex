using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with user-group relationships in the database.
    /// </summary>
    public interface IUserGroupRepository : IGenericRepository<UserGroup>
    {
        /// <summary>
        /// Gets the total number of groups a user belongs to.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The total number of groups for the user.</returns>
        Task<int> GetUserGroupCountAsync(Guid userId, RequestStatus status, CancellationToken cancellationToken);

        Task<bool> IsUserInGroupAsync(Guid userId, Guid groupId, RequestStatus? status = null, 
            CancellationToken cancellationToken = default);
        Task<bool> IsGroupPrivateAsync(Guid groupId, CancellationToken cancellationToken);
        Task<bool> IsUserBannedAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);
    }
}