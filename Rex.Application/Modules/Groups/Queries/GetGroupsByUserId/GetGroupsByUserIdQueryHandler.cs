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
    IDistributedCache distributedCache,
    IUserRepository userRepository
) : IQueryHandler<GetGroupsByUserIdQuery, PagedResult<GroupDetailsDto>>
{
    public async Task<ResultT<PagedResult<GroupDetailsDto>>> Handle(GetGroupsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogError("GetGroupsByUserIdQuery: Request is null.");
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("400", "Oops! We didn’t receive any data to fetch groups."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("GetGroupsByUserIdQuery: User with ID {UserId} not found.", request.UserId);
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("404", "We couldn’t find your user account."));
        }

        var result = await distributedCache.GetOrCreateAsync(
            $"get-groups-by-userid-{request.UserId}-{request.pageNumber}-{request.pageSize}",
            async () => await groupRepository.GetGroupsByUserIdAsync(
                request.UserId,
                request.pageNumber,
                request.pageSize,
                cancellationToken
            ),
            logger,
            cancellationToken: cancellationToken
        );

        var elements = result.Items
            .Select(c => new GroupDetailsDto(
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
            logger.LogWarning("GetGroupsByUserIdQuery: No groups found for user {UserId}.", request.UserId);
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("404", "You haven’t joined any groups yet."));
        }

        var pagedResult = new PagedResult<GroupDetailsDto>(
            elements.ToList(),
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );

        logger.LogInformation("GetGroupsByUserIdQuery: Returning {Count} groups for user {UserId}.",
            pagedResult.TotalItems, request.UserId);

        return ResultT<PagedResult<GroupDetailsDto>>.Success(pagedResult);
    }
}