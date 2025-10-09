using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class ReactionRepository(RexContext context) : GenericRepository<Reaction>(context), IReactionRepository
{
    public async Task<int> CountLikesAsync(Guid targetId, ReactionTargetType targetType,
        CancellationToken cancellationToken) =>
        await context.Set<Reaction>()
            .AsNoTracking()
            .CountAsync(r => r.TargetId == targetId
                             && r.TargetType == targetType.ToString()
                             && r.Like,
                cancellationToken);

    public async Task<Reaction> HasLikedAsync(Guid targetId, Guid userId,
        CancellationToken cancellationToken) =>
        await context.Set<Reaction>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.TargetId == targetId && c.UserId == userId, cancellationToken);

    public async Task<Dictionary<Guid, int>> GetLikesCountByPostIdsAsync(IEnumerable<Guid> Ids, TargetType targetType,
        CancellationToken cancellationToken) =>
        await context.Set<Reaction>()
            .Where(c => Ids.Contains(c.TargetId) && c.Like && c.TargetType == targetType.ToString())
            .AsNoTracking()
            .GroupBy(c => c.TargetId)
            .Select(g => new { targetId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.targetId, g => g.Count, cancellationToken);
}