using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Pagination;
using Rex.Enum;

namespace Rex.Application.Modules.Challenges.Queries.GetChallengesByUser;

public record GetChallengesByUserQuery(
    Guid UserId,
    UserChallengeStatus Status,
    int PageNumber,
    int PageSize
    ): IQuery<PagedResult<ChallengeUserDetailsDto>>;