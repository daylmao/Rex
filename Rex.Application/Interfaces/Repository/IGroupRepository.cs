using Rex.Application.DTOs;
using Rex.Application.Pagination;
using Rex.Enum;
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
        Task<PagedResult<Group>> GetGroupsByUserIdAsync(Guid userId, int page, int size, CancellationToken cancellationToken);
        
        Task<PagedResult<Group>> GetGroupsPaginatedAsync(Guid userId, int page, int size, CancellationToken cancellationToken);
        Task<Group> GetGroupByIdAsync(Guid groupId, CancellationToken cancellationToken);
        
    }
}