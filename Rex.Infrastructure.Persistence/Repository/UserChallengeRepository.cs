using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserChallengeRepository(RexContext context)
    : GenericRepository<UserChallenge>(context), IUserChallengeRepository
{
    public async Task<int> GetChallengesCountByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await context.Set<UserChallenge>()
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .CountAsync(cancellationToken);

    public async Task<UserChallenge> GetByUserAndChallengeAsync(Guid userId, Guid challengeId,
        CancellationToken cancellationToken) =>
        await context.Set<UserChallenge>()
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ChallengeId == challengeId, cancellationToken);
}