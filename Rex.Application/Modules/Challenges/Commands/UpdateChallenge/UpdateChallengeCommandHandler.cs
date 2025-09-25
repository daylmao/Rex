using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Challenges.Commands.UpdateChallenge;

public class UpdateChallengeCommandHandler(
    ILogger<UpdateChallengeCommandHandler> logger,
    IChallengeRepository challengeRepository
): ICommandHandler<UpdateChallengeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateChallengeCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("UpdateChallenge: Received an empty request.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "We couldn’t process your request because it was empty. Please provide the challenge details and try again."));
        }
        
        var challenge = await challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);
        if (challenge is null)
        {
            logger.LogWarning("UpdateChallenge: Challenge with ID {ChallengeId} could not be found.", request.ChallengeId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", $"We couldn’t find the challenge. Please make sure the ID is correct."));
        }
        
        challenge.Title = request.Title;
        challenge.Description = request.Description;
        challenge.Duration = request.Duration;
        challenge.Status = request.Status;
        challenge.UpdatedAt = DateTime.UtcNow;

        await challengeRepository.UpdateAsync(challenge, cancellationToken);

        logger.LogInformation("UpdateChallenge: Challenge with ID {ChallengeId} was updated successfully.", request.ChallengeId);
        return ResultT<ResponseDto>.Success(new ("Your challenge has been updated successfully!"));
    }
}