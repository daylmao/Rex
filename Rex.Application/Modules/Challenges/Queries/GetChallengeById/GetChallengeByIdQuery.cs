using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.Challenges.Queries.GetChallengeById;

public record GetChallengeByIdQuery(
    Guid ChallengeId,
    Guid GroupId,
    Guid UserId
    ): IQuery<ChallengeDetailsDto>;