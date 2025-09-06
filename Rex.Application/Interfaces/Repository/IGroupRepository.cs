using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IGroupRepository: IGenericRepository<Group>
{
    Task<PagedResult<Group>> GetGroupsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    
    Task<bool> GroupExistAsync(Guid groupId, CancellationToken cancellationToken);
    
    Task<int> GetGroupCountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}