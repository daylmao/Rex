using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Challenge;  
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Challenges.Queries.GetChallengeById;

public class GetChallengeByIdQueryHandler(
    ILogger<GetChallengeByIdQueryHandler> logger,
    IGroupRepository groupRepository,
    IChallengeRepository challengeRepository
    ): IQueryHandler<GetChallengeByIdQuery, ChallengeDetailsDto>
{
    public async Task<ResultT<ChallengeDetailsDto>> Handle(GetChallengeByIdQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("The request to get a challenge was empty");
            return ResultT<ChallengeDetailsDto>.Failure(Error.Failure("400", "The request is invalid"));
        }

        var group = await groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("No group found with id {GroupId}", request.GroupId);
            return ResultT<ChallengeDetailsDto>.Failure(Error.NotFound("404", "The specified group was not found"));
        }

        var challenge = await challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);
        if (challenge is null || challenge.GroupId != request.GroupId)
        {
            logger.LogWarning("Challenge {ChallengeId} does not exist in group {GroupId}", request.ChallengeId, request.GroupId);
            return ResultT<ChallengeDetailsDto>.Failure(Error.NotFound("404", "The challenge does not exist in the selected group"));
        }

        var joined = await challengeRepository.UserAlreadyJoined(request.UserId, request.ChallengeId, cancellationToken);
        if (!joined)
        {
            logger.LogWarning("User {UserId} tried to access challenge {ChallengeId} without joining", request.UserId, request.ChallengeId);
            return ResultT<ChallengeDetailsDto>.Failure(Error.Failure("403", "You must join the challenge before posting"));
        }

        var challengeDetails = new ChallengeDetailsDto
        (
            challenge.Id,
            challenge.Title,
            challenge.Description,
            challenge.Status,
            challenge.CoverPhoto,
            challenge.Duration
        );
        
        return ResultT<ChallengeDetailsDto>.Success(challengeDetails);
    }
}