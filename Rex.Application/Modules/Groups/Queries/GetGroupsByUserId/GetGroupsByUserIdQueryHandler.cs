using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Groups.Queries.GetGroupsByUserId;

public class GetGroupsByUserIdQueryHandler(
    ILogger<GetGroupsByUserIdQueryHandler> logger,
    IGroupRepository groupRepository,
    IDistributedCache distributedCache
    ) : IQueryHandler<GetGroupsByUserIdQuery, PagedResult<GroupDetailsDto>>
{
    public async Task<ResultT<PagedResult<GroupDetailsDto>>> Handle(GetGroupsByUserIdQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogError("Request to get groups by user is null.");
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("400", "The request cannot be empty."));
        }

        if (request.UserId == null)
        {
            logger.LogError("The request did not include a valid user ID.");
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("400", "A valid user ID must be provided."));
        }

        var result = await distributedCache.GetOrCreateAsync(
            $"get-groups-by-userid-{request.UserId.ToString().ToLower()}",
            async () => await groupRepository.GetGroupsByUserIdAsync(
                request.UserId,
                request.pageNumber,
                request.pageSize,
                cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );

        var elements = result.Items
            .Select(c => new GroupDetailsDto
            (
               ProfilePicture: c.ProfilePhoto,
               CoverPicture: c.CoverPhoto ?? string.Empty,
               Title: c.Title,
               Description: c.Description,
               Visibility: c.Visibility,
               MemberCount: c.UserGroups.Count,
               IsJoined: c.UserGroups.Any(ug => ug.UserId == request.UserId)
            ));

        if (!elements.Any())
        {
            logger.LogWarning("No groups were found for user {UserId}.", request.UserId);
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("404", "This user is not part of any groups yet."));
        }

        var pagedResult = new PagedResult<GroupDetailsDto>(
            elements.ToList(),
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );

        logger.LogInformation("Returning {Count} groups for user {UserId}.", pagedResult.TotalItems, request.UserId);

        return ResultT<PagedResult<GroupDetailsDto>>.Success(pagedResult);
    }
}
