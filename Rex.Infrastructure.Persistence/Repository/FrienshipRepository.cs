using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class FrienshipRepository(RexContext context): GenericRepository<FriendShip>(context), IFriendShipRepository
{
    public async Task<PagedResult<FriendShip>> GetFriendShipRequestsByUserIdAsync(Guid userId, int page, int size,
        RequestStatus status, CancellationToken cancellationToken)
    {
        var query = context.Set<FriendShip>()
            .Where(f => f.TargetUserId == userId && f.Status == status.ToString());
        
        var total = await query.CountAsync(cancellationToken);
       
       var friendships = await query
           .Where(f => f.TargetUserId == userId && f.Status == status.ToString())
           .OrderByDescending(c => c.CreatedAt)
           .Skip((page - 1) * size)
           .Take(size)
           .ToListAsync(cancellationToken);

       return new PagedResult<FriendShip>(friendships, total, page, size);
    }

    public async Task<bool> FriendShipExistAsync(Guid requesterId, Guid targetUserId,
        CancellationToken cancellationToken) =>
        await ValidateAsync(f => f.RequesterId == requesterId && f.TargetUserId == targetUserId, cancellationToken);
}