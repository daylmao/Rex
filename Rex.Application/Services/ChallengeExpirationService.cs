using Microsoft.Extensions.Logging;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Services;

public class ChallengeExpirationService(
    ILogger<ChallengeExpirationService> logger,
    IChallengeRepository challengeRepository
) : IChallengeExpirationService
{
    public async Task<ResultT<ResponseDto>> MarkChallengeExpired(CancellationToken cancellationToken)
    {
        var challenges = await challengeRepository.GetExpiredChallenges(cancellationToken);

        if (challenges is null || !challenges.Any())
        {
            logger.LogInformation("No expired challenges found to process");
            return ResultT<ResponseDto>.Success(new ResponseDto("No expired challenges found at this time"));
        }

        foreach (var challenge in challenges)
        {
            challenge.Status = ChallengeStatus.Ended.ToString();
            challenge.UpdatedAt = DateTime.UtcNow;
        }

        await challengeRepository.UpdateRangeAsync(challenges, cancellationToken);

        logger.LogInformation("Successfully marked {Count} challenge(s) as expired", challenges.Count());

        return ResultT<ResponseDto>.Success(
            new ResponseDto($"Successfully marked {challenges.Count()} challenge(s) as expired")
        );
    }
}