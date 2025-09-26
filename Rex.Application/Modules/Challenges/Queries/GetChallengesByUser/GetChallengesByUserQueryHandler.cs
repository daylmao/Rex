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
    ): IQueryHandler<GetChallengesByUserQuery, PagedResult<ChallengeUserDetailsDto>>
{
    public async Task<ResultT<PagedResult<ChallengeUserDetailsDto>>> Handle(GetChallengesByUserQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("");
            return ResultT<PagedResult<ChallengeUserDetailsDto>>.Failure(Error.Failure("400", "Request is invalid. Please try again."));
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with id {UserId} not found", request.UserId);
            return ResultT<PagedResult<ChallengeUserDetailsDto>>.Failure(Error.Failure("404", "User not found. Please check your account details."));
        }

        var result = await cache.GetOrCreateAsync(
            $"get-challenges-by-user-{request.UserId}-{request.Status}-{request.PageNumber}-{request.PageSize}",
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
        
        var elements = result.Items.Select(challenge => new ChallengeUserDetailsDto
        (
            challenge.Group.ProfilePhoto,
            challenge.Group.ProfilePhoto,
            challenge.Title,
            challenge.Description,
            challenge.UserChallenges!.FirstOrDefault(uc => uc.UserId == request.UserId)!.Status,
            challenge.Duration,
            challenge.Group.Title,
            challenge.UserChallenges.Select(c => c.User.ProfilePhoto)
                .Where(photo => !string.IsNullOrEmpty(photo))
                .Take(5)
                .ToList(), 
            challenge.UserChallenges
                .Count(c => c.Status == UserChallengeStatus.Enrolled.ToString())
        )).ToList();

        if (!elements.Any())
        {
            logger.LogWarning("No challenges found for user with ID {UserId} and status {Status}.", request.UserId, request.Status);
            return ResultT<PagedResult<ChallengeUserDetailsDto>>.Failure(Error.NotFound("404", "No challenges found for the specified user and status."));
        }
        
        var pagedResult = new PagedResult<ChallengeUserDetailsDto>(
            elements,
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );
        return ResultT<PagedResult<ChallengeUserDetailsDto>>.Success(pagedResult);
    }
}