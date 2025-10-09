using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserChallengeRepository(RexContext context): GenericRepository<UserChallenge>(context), IUserChallengeRepository
{
    public async Task<int> GetChallengesCountByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await context.Set<UserChallenge>()
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .CountAsync(cancellationToken);
    public Task<bool> AnyUserCompletedChallengeAsync(Guid challengeId, CancellationToken cancellationToken)
    {
        return context.Set<UserChallenge>()
            .AnyAsync(uc => uc.ChallengeId == challengeId && uc.Status == UserChallengeStatus.Completed.ToString(), cancellationToken);
    }
}   