using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserChallengeRepository(RexContext context): GenericRepository<UserChallenge>(context), IUserChallengeRepository
{
    public async Task<int> GetChallengesCountByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.UserChallenges.Any(uc => uc.UserId == userId))
            .CountAsync(cancellationToken);

}   