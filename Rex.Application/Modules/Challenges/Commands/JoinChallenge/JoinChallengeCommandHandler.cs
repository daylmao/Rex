using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Modules.Challenges.Commands.JoinChallenge;

public class JoinChallengeCommandHandler(
    ILogger<JoinChallengeCommandHandler> logger,
    IChallengeRepository challengeRepository,
    IGroupRepository groupRepository,
    IUserRepository userRepository,
    IUserChallengeRepository userChallengeRepository
    ): ICommandHandler<JoinChallengeCommand, ResponseDto>
{
        public async Task<ResultT<ResponseDto>> Handle(JoinChallengeCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("JoinChallengeCommand request was null");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Request is invalid. Please try again."));
        }
        
        var challenge = await challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);
        if (challenge is null)
        {
            logger.LogWarning("Challenge with id {ChallengeId} not found", request.ChallengeId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Sorry, we couldn't find the challenge."));
        }

        if (challenge.Status != ChallengeStatus.Active.ToString())
        {
            logger.LogWarning("Challenge with id {ChallengeId} is not active", request.ChallengeId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "This challenge is not active at the moment."));
        }

        if ((challenge.CreatedAt + challenge.Duration) < DateTime.UtcNow)
        {
            logger.LogWarning("Challenge with id {ChallengeId} has expired", request.ChallengeId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "The time to join this challenge has expired."));
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with id {UserId} not found", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "User not found. Please check your account details."));
        }
        
        UserChallenge userChallenge = new()
        {
            ChallengeId = challenge.Id,
            UserId = user.Id,
            Status = UserChallengeStatus.Enrolled.ToString()
        };
        
        await userChallengeRepository.CreateAsync(userChallenge, cancellationToken);
        logger.LogInformation("User with id {UserId} joined challenge with id {ChallengeId}", request.UserId, request.ChallengeId);
        return ResultT<ResponseDto>.Success(new ResponseDto("You've successfully joined the challenge! Good luck!"));
    }

}