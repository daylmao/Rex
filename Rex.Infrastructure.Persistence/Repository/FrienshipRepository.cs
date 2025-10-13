using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class FrienshipRepository(RexContext context) : GenericRepository<FriendShip>(context), IFriendShipRepository
{
    public async Task<PagedResult<FriendShip>> GetFriendShipRequestsByUserIdAsync(
        Guid userId, int page, int size, RequestStatus status, CancellationToken cancellationToken)
    {
        var query = context.Set<FriendShip>()
            .Where(f => f.TargetUserId == userId && f.Status == status.ToString())
            .Select(f => new FriendShip
            {
                Id = f.Id,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                Requester = new User
                {
                    Id = f.RequesterId,
                    FirstName = f.Requester.FirstName,
                    LastName = f.Requester.LastName,
                    ProfilePhoto = f.Requester.ProfilePhoto
                },
                TargetUser = new User
                {
                    Id = f.TargetUserId,
                    FirstName = f.TargetUser.FirstName,
                    LastName = f.TargetUser.LastName,
                    ProfilePhoto = f.TargetUser.ProfilePhoto
                }
            });

        var total = await query.CountAsync(cancellationToken);

        var friendships = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<FriendShip>(friendships, total, page, size);
    }

    public async Task<bool> FriendShipExistAsync(Guid requesterId, Guid targetUserId,
        CancellationToken cancellationToken) =>
        await ValidateAsync(
            f => f.RequesterId == requesterId && f.TargetUserId == targetUserId &&
                f.Status == RequestStatus.Pending.ToString() || f.Status == RequestStatus.Accepted.ToString(),
            cancellationToken);

    public async Task<FriendShip> GetFriendShipBetweenUsersAsync(Guid RequesterId, Guid TargetUserId,
        CancellationToken cancellationToken) =>
        await context.Set<FriendShip>()
            .Where(c => c.RequesterId == RequesterId && c.TargetUserId == TargetUserId &&
                        c.Status == RequestStatus.Pending.ToString())
            .FirstOrDefaultAsync(cancellationToken);
}