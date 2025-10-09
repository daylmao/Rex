using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Challenges.Queries.GetChallengesByStatus;

public class GetChallengesByStatusQueryHandler(
    ILogger<GetChallengesByStatusQueryHandler> logger,
    IChallengeRepository challengeRepository,
    IGroupRepository groupRepository,
    IDistributedCache distributedCache
) : IQueryHandler<GetChallengesByStatusQuery, PagedResult<ChallengeGroupDetailsDto>>
{
    public async Task<ResultT<PagedResult<ChallengeGroupDetailsDto>>> Handle(GetChallengesByStatusQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request for challenges by status.");
            return ResultT<PagedResult<ChallengeGroupDetailsDto>>.Failure(Error.Failure("400",
                "Oops! The request cannot be empty."));
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group not found for retrieving challenges.");
            return ResultT<PagedResult<ChallengeGroupDetailsDto>>.Failure(Error.NotFound("404",
                "Hmm, we couldn't find the specified group."));
        }

        logger.LogInformation(
            "Fetching challenges for group with status {Status}. Page {PageNumber}, PageSize {PageSize}.",
            request.Status, request.PageNumber, request.PageSize);

        var result = await distributedCache.GetOrCreateAsync(
            $"challenges:group:{request.GroupId}:status:{request.Status}:page:{request.PageNumber}:size:{request.PageSize}",
            async () => await challengeRepository.GetChallengesPaginatedByGroupIdAndStatusAsync(
                request.GroupId,
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
            logger.LogInformation("No challenges found for group {GroupId} with status {Status}", 
                request.GroupId, request.Status);
            return ResultT<PagedResult<ChallengeGroupDetailsDto>>.Success(
                new PagedResult<ChallengeGroupDetailsDto>([], result.TotalItems, result.ActualPage, result.TotalPages)
            );
        }

        var elements = result.Items
            .Select(c => new ChallengeGroupDetailsDto(
                c.Title,
                c.Description,
                c.Status.ToString(),
                c.CoverPhoto,
                c.Duration,
                c.UserChallenges
                    .Select(uc => uc.User.ProfilePhoto)
                    .Where(pp => !string.IsNullOrEmpty(pp))
                    .Take(5)
                    .ToList(),
                c.UserChallenges.Count(uc => uc.Status == UserChallengeStatus.Enrolled.ToString())
            ))
            .ToList();

        var pagedResult = new PagedResult<ChallengeGroupDetailsDto>(
            elements,
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );

        logger.LogInformation("Successfully retrieved {Count} challenges for group {GroupId}", 
            elements.Count, request.GroupId);
        return ResultT<PagedResult<ChallengeGroupDetailsDto>>.Success(pagedResult);
    }
}