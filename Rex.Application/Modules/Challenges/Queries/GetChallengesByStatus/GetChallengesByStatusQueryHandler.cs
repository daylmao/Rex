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
    IFileRepository fileRepository,
    IDistributedCache distributedCache
    ): IQueryHandler<GetChallengesByStatusQuery, PagedResult<ChallengeGroupDetailsDto>>
{
    public async Task<ResultT<PagedResult<ChallengeGroupDetailsDto>>> Handle(GetChallengesByStatusQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Request is null. Cannot process GetChallengesByStatus query.");
            return ResultT<PagedResult<ChallengeGroupDetailsDto>>.Failure(Error.Failure("400", "The request cannot be empty."));
        }
        
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group with ID {GroupId} was not found.", request.GroupId);
            return ResultT<PagedResult<ChallengeGroupDetailsDto>>.Failure(Error.NotFound("404", $"Group with ID {request.GroupId} does not exist."));
        }

        logger.LogInformation("Fetching challenges for GroupId {GroupId} with Status {Status}. Page {PageNumber}, PageSize {PageSize}.",
            request.GroupId, request.Status, request.PageNumber, request.PageSize);

        var result = await distributedCache.GetOrCreateAsync(
            $"get-challenges-by-status-{request.GroupId}-{request.Status}-{request.PageNumber}-{request.PageSize}",
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

        if (result is null)
        {
            logger.LogWarning("No challenges found for GroupId {GroupId} with Status {Status}.", request.GroupId, request.Status);
            return ResultT<PagedResult<ChallengeGroupDetailsDto>>.Failure(Error.NotFound("404", "No challenges available for the specified criteria."));
        }
        
        var file = await fileRepository.GetFileByEntityAndTypeAsync(request.GroupId, TargetType.Challenge, cancellationToken);

        var elements = result.Items
            .Select(c => new ChallengeGroupDetailsDto(
                c.Title,
                c.Description,
                c.Status.ToString(),
                file?.Url ?? string.Empty,
                c.Duration,
                c.UserChallenges
                    .Select(uc => uc.User.ProfilePhoto)
                    .Where(pp => !string.IsNullOrEmpty(pp))
                    .Take(5)
                    .ToList(),
                c.UserChallenges.Count(uc => uc.Status == UserChallengeStatus.Enrolled.ToString())
            ));

        if (!elements.Any())
        {
            logger.LogWarning("Challenges were retrieved but no detailed data is available for GroupId {GroupId} with Status {Status}.", request.GroupId, request.Status);
            return ResultT<PagedResult<ChallengeGroupDetailsDto>>.Failure(Error.NotFound("404", "Challenges were found but no detailed information could be retrieved."));
        }
        
        var pagedResult = new PagedResult<ChallengeGroupDetailsDto>(
            elements,
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );
        
        logger.LogInformation("Successfully retrieved {Count} challenges for GroupId {GroupId} with Status {Status}.", elements.Count(), request.GroupId, request.Status);
        return ResultT<PagedResult<ChallengeGroupDetailsDto>>.Success(pagedResult);
    }
}
