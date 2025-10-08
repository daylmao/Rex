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
            logger.LogWarning("Received empty request to update a challenge.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! Your request was empty. Please provide the challenge details and try again."));
        }

        var challenge = await challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);
        if (challenge is null)
        {
            logger.LogWarning("Challenge not found when attempting to update.");
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Hmm, we couldn’t find the challenge you’re trying to update."));
        }

        challenge.Title = request.Title;
        challenge.Description = request.Description;
        challenge.Duration = request.Duration;
        challenge.Status = request.Status.ToString();
        challenge.UpdatedAt = DateTime.UtcNow;

        await challengeRepository.UpdateAsync(challenge, cancellationToken);

        logger.LogInformation("Challenge updated successfully.");
        return ResultT<ResponseDto>.Success(new ResponseDto("Your challenge has been updated successfully!"));
    }

}