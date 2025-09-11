using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class ChallengeRepository(RexContext context): GenericRepository<Challenge>(context), IChallengeRepository
{
    public async Task<PagedResult<Challenge>> GetChallengesPaginatedByGroupIdAndStatusAsync(Guid groupId, int page, int size, ChallengeStatus status,
        CancellationToken cancellationToken)
    {
        var total = await context.Set<Challenge>()
            .AsNoTracking()
            .Where(g => g.GroupId == groupId && g.Status == status.ToString())
            .CountAsync(cancellationToken);

        var challenges = await context.Set<Challenge>()
            .Where(g => g.GroupId == groupId && g.Status == status.ToString())
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Challenge>(challenges, total, page, size);
    }

    public async Task<PagedResult<Challenge>> GetChallengesPaginatedByUserParticipationGroupAndStatusAsync(Guid userId, int page, int size,
        Guid groupId, UserChallengeStatus status,
        CancellationToken cancellationToken)
    {
        var total = await context.Set<Challenge>()
            .AsNoTracking()
            .Where(c => c.Status == status.ToString() && c.GroupId == groupId &&
                        c.UserChallenges.Any(g => g.UserId == userId)).CountAsync(cancellationToken);
        
        var challenges = await context.Set<Challenge>()
            .Where(c => c.Status == status.ToString() && c.GroupId == groupId &&
                        c.UserChallenges.Any(g => g.UserId == userId))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Challenge>(challenges, total, page, size);
    }

    public async Task<PagedResult<Challenge>> GetChallengesPaginatedByUserIdAndStatusAsync(Guid userId, int page, int size,
        UserChallengeStatus status,
        CancellationToken cancellationToken)
    {
        var total = await context.Set<Challenge>()
            .Where(c => c.Status == status.ToString() && c.UserChallenges.Any(g => g.UserId == userId))
            .CountAsync(cancellationToken);
        
        var challenges = await context.Set<Challenge>()
            .Where(c => c.Status == status.ToString() && c.UserChallenges.Any(g => g.UserId == userId))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Challenge>(challenges, total, page, size);
    }

    public async Task<bool> UserAlreadyJoined(Guid userId, Guid challengeId, CancellationToken cancellationToken) =>
        await ValidateAsync( c => c.UserChallenges.Any( a => a.UserId == userId && a.ChallengeId == challengeId ), cancellationToken);
}