using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with friendships in the database.
    /// </summary>
    public interface IFriendShipRepository : IGenericRepository<FriendShip>
    {
        /// <summary>
        /// Gets a paginated list of friendship requests for a specific user filtered by request status.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="status">The status of the friendship requests.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of friendship requests.</returns>
        Task<PagedResult<FriendShip>> GetFriendShipRequestsByUserIdAsync(Guid userId, int page, int size,
            RequestStatus status, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a friendship exists between two users.
        /// </summary>
        /// <param name="requesterId">The ID of the user who sent the request.</param>
        /// <param name="targetUserId">The ID of the target user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the friendship exists; otherwise false.</returns>
        Task<bool> FriendShipExistAsync(Guid requesterId, Guid targetUserId, CancellationToken cancellationToken);
        
        Task<FriendShip> GetFriendShipBetweenUsersAsync(Guid RequesterId, Guid TargetUserId, CancellationToken cancellationToken);
    }
}