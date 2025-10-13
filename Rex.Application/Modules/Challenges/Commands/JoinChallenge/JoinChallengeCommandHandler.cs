using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Modules.Challenges.Commands.JoinChallenge;

public class JoinChallengeCommandHandler(
    ILogger<JoinChallengeCommandHandler> logger,
    IChallengeRepository challengeRepository,
    IUserRepository userRepository,
    IUserChallengeRepository userChallengeRepository
) : ICommandHandler<JoinChallengeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(JoinChallengeCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request to join a challenge.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "Oops! The request is invalid. Please try again."));
        }

        var challenge = await challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);
        if (challenge is null)
        {
            logger.LogWarning("Challenge not found when attempting to join.");
            return ResultT<ResponseDto>.Failure(Error.Failure("404",
                "Sorry, we couldn't find the challenge you want to join."));
        }

        if (challenge.Status != ChallengeStatus.Active.ToString())
        {
            logger.LogWarning("Challenge is not active.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "This challenge is not active right now."));
        }

        if ((challenge.CreatedAt + challenge.Duration) < DateTime.UtcNow)
        {
            logger.LogWarning("Challenge has expired.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "The time to join this challenge has passed."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found when attempting to join challenge.");
            return ResultT<ResponseDto>.Failure(Error.Failure("404",
                "We couldn't find your account. Please check your details."));
        }

        UserChallenge userChallenge = new()
        {
            ChallengeId = challenge.Id,
            UserId = user.Id,
            Status = UserChallengeStatus.Enrolled.ToString()
        };

        await userChallengeRepository.CreateAsync(userChallenge, cancellationToken);

        logger.LogInformation("User joined the challenge successfully.");
        return ResultT<ResponseDto>.Success(new ResponseDto("You've successfully joined the challenge! Good luck!"));
    }
}