using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IChallengeRepository: IGenericRepository<Challenge>
{
    Task<PagedResult<Challenge>> GetChallengesPaginatedByGroupIdAndStatusAsync(Guid groupId, int page, int size,
        ChallengeStatus status,
        CancellationToken cancellationToken);
    
    Task<PagedResult<Challenge>> GetChallengesPaginatedByUserParticipationGroupAndStatusAsync(Guid userId, int page, int size,
        Guid groupId, UserChallengeStatus status,
        CancellationToken cancellationToken);
    
    Task<PagedResult<Challenge>> GetChallengesPaginatedByUserIdAndStatusAsync(Guid userId, int page, int size,
        UserChallengeStatus status,
        CancellationToken cancellationToken);
    
    Task<bool> UserAlreadyJoined(Guid userId, Guid challengeId, CancellationToken cancellationToken);
}