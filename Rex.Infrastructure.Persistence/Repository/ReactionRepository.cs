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

    public async Task<Reaction> HasLikedAsync(Guid targetId, Guid userId, ReactionTargetType targetType,
        CancellationToken cancellationToken) =>
        await context.Set<Reaction>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.TargetId == targetId && c.UserId == userId && c.TargetType == targetType.ToString(),
                cancellationToken);
}