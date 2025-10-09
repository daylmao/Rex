using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Challenges.Queries.GetChallengesByUser;

public class GetChallengesByUserQueryHandler(
    ILogger<GetChallengesByUserQueryHandler> logger,
    IChallengeRepository challengeRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : IQueryHandler<GetChallengesByUserQuery, PagedResult<ChallengeUserDetailsDto>>
{
    public async Task<ResultT<PagedResult<ChallengeUserDetailsDto>>> Handle(GetChallengesByUserQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received invalid request for retrieving challenges.");
            return ResultT<PagedResult<ChallengeUserDetailsDto>>.Failure(Error.Failure("400",
                "Oops! The request seems invalid. Please try again."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found when fetching challenges.");
            return ResultT<PagedResult<ChallengeUserDetailsDto>>.Failure(Error.Failure("404",
                "Hmm, we couldn't find your account. Please check your details."));
        }

        var result = await cache.GetOrCreateAsync(
            $"challenges:user:{request.UserId}:status:{request.Status}:page:{request.PageNumber}:size:{request.PageSize}",
            async () => await challengeRepository.GetChallengesPaginatedByUserIdAndStatusAsync(
                request.UserId,
                request.PageNumber,
                request.PageSize,
                request.Status,
                cancellationToken
            ),
            logger,
            cancellationToken: cancellationToken
        );

        if (!result.Items.Any())
        {
            logger.LogInformation("No challenges found for user {UserId} with status {Status}", 
                request.UserId, request.Status);
            return ResultT<PagedResult<ChallengeUserDetailsDto>>.Success(
                new PagedResult<ChallengeUserDetailsDto>([], result.TotalItems, result.ActualPage, result.TotalPages)
            );
        }

        var elements = result.Items.Select(challenge =>
        {
            var userChallenge = challenge.UserChallenges?.FirstOrDefault(uc => uc.UserId == request.UserId);
            var userChallengeStatus = userChallenge?.Status ?? string.Empty;

            return new ChallengeUserDetailsDto
            (
                challenge.Group.ProfilePhoto,
                challenge.CoverPhoto,
                challenge.Title,
                challenge.Description,
                userChallengeStatus,
                challenge.Duration,
                challenge.Group.Title,
                challenge.UserChallenges?
                    .Select(c => c.User.ProfilePhoto)
                    .Where(photo => !string.IsNullOrEmpty(photo))
                    .Take(5)
                    .ToList() ?? [],
                challenge.UserChallenges?
                    .Count(c => c.Status == UserChallengeStatus.Enrolled.ToString()) ?? 0
            );
        }).ToList();

        var pagedResult = new PagedResult<ChallengeUserDetailsDto>(
            elements,
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );

        logger.LogInformation("Successfully retrieved {Count} challenges for user {UserId}", 
            elements.Count, request.UserId);
        return ResultT<PagedResult<ChallengeUserDetailsDto>>.Success(pagedResult);
    }
}