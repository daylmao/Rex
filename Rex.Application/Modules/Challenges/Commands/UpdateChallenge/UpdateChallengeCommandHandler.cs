using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Challenges.Commands.UpdateChallenge;

public class UpdateChallengeCommandHandler(
    ILogger<UpdateChallengeCommandHandler> logger,
    IChallengeRepository challengeRepository,
    IGroupRepository groupRepository,
    IDistributedCache cache
): ICommandHandler<UpdateChallengeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateChallengeCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request to update a challenge.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! Your request was empty. Please provide the challenge details and try again."));
        }

        var challenge = await challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);
        if (challenge is null)
        {
            logger.LogWarning("Challenge not found when attempting to update.");
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Hmm, we couldn’t find the challenge you’re trying to update."));
        }
        
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group not found for the challenge being updated.");
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "The group associated with this challenge seems to be missing."));
        }
        
        var belongsToGroup = await challengeRepository.ChallengeBelongsToGroup(
            request.GroupId, request.ChallengeId, cancellationToken);

        if (!belongsToGroup)
        {
            logger.LogWarning("Challenge {ChallengeId} doesn't belong to group {GroupId}.",
                request.ChallengeId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "This challenge doesn't belong to the group you're posting in."));
        }

        challenge.Title = request.Title;
        challenge.Description = request.Description;
        challenge.Duration = request.Duration;
        challenge.Status = request.Status.ToString();
        challenge.UpdatedAt = DateTime.UtcNow;

        await challengeRepository.UpdateAsync(challenge, cancellationToken);

        await cache.IncrementVersionAsync("challenge", request.ChallengeId, logger, cancellationToken);
        
        logger.LogInformation("Challenge updated successfully.");
        return ResultT<ResponseDto>.Success(new ResponseDto("Your challenge has been updated successfully!"));
    }

}