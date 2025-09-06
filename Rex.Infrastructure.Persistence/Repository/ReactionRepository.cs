using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class ReactionRepository(RexContext context): GenericRepository<Reaction>(context), IReactionRepository
{
    public async Task<int> GetReactionCountByPostIdAsync(Guid postId, ReactionTargetType targetType, CancellationToken cancellationToken) =>
        await context.Set<Reaction>()
            .AsNoTracking()
            .Where(c => c.TargetId == postId && c.TargetType == targetType.ToString())
            .CountAsync(cancellationToken);
    
    public async Task<int> GetReactionCountByCommentIdAsync(Guid commentId, ReactionTargetType targetType, CancellationToken cancellationToken) =>
        await context.Set<Reaction>()
            .AsNoTracking()
            .Where(c => c.TargetId == commentId && c.TargetType == targetType.ToString())
            .CountAsync(cancellationToken);
}