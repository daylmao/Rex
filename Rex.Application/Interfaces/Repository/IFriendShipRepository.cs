using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IFriendShipRepository: IGenericRepository<FriendShip>
{
    Task<PagedResult<FriendShip>> GetFriendShipRequestsByUserIdAsync(Guid userId, int page, int size,
        RequestStatus status, CancellationToken cancellationToken);
    
    Task<bool> FriendShipExistAsync(Guid requesterId, Guid targetUserId, CancellationToken cancellationToken);
    
}