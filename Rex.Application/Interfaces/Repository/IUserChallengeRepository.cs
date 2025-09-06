using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IUserChallengeRepository: IGenericRepository<UserChallenge>
{
    Task<int> GetChallengesCountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}