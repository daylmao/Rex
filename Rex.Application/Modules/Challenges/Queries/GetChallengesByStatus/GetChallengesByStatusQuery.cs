using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Challenge;  
using Rex.Application.Pagination;
using Rex.Enum;

namespace Rex.Application.Modules.Challenges.Queries.GetChallengesByStatus;

public record GetChallengesByStatusQuery(
    Guid GroupId,
    ChallengeStatus Status,
    int PageNumber,
    int PageSize
    ): IQuery<PagedResult<ChallengeGroupDetailsDto>>;