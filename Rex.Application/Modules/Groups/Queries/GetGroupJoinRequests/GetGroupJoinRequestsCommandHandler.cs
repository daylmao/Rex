using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Queries.GetGroupJoinRequests;

public class GetGroupJoinRequestsCommandHandler(
    ILogger<GetGroupJoinRequestsCommandHandler> logger,
    IUserGroupRepository userGroupRepository,
    IGroupRepository groupRepository,
    IDistributedCache distributedCache
) : IQueryHandler<GetGroupJoinRequestsCommand, PagedResult<UserGroupRequestDto>>
{
    public async Task<ResultT<PagedResult<UserGroupRequestDto>>> Handle(GetGroupJoinRequestsCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received a null GetGroupJoinRequestsCommand.");
            return ResultT<PagedResult<UserGroupRequestDto>>.Failure(
                Error.Failure("400", "We couldn’t process your request. Please try again.")
            );
        }

        var group = await groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("No group found with ID {GroupId}.", request.GroupId);
            return ResultT<PagedResult<UserGroupRequestDto>>.Failure(
                Error.NotFound("404", "The group you’re looking for doesn’t exist.")
            );
        }

        var result = await distributedCache.GetOrCreateAsync(
            $"get-group-join-requests-{request.GroupId}-{request.PageNumber}-{request.PageSize}-{request.SearchTerm}",
            async () => await userGroupRepository.GetGroupRequestsAsync(
                request.GroupId, 
                RequestStatus.Pending, 
                request.SearchTerm,
                request.PageNumber, 
                request.PageSize, 
                cancellationToken
            ),
            logger,
            cancellationToken: cancellationToken
        );

        var elements = result.Items
            .Select(c => new UserGroupRequestDto(
                c.User.FirstName,
                c.User.LastName,
                c.User.ProfilePhoto,
                c.Status?.ToString() ?? "Unknown",
                DateTime.UtcNow - c.RequestedAt
            ));

        if (!elements.Any())
        {
            logger.LogInformation("No join requests found for group with ID {GroupId}.", request.GroupId);
            return ResultT<PagedResult<UserGroupRequestDto>>.Failure(
                Error.NotFound("404", "This group doesn’t have any pending join requests yet.")
            );
        }

        var pagedResult = new PagedResult<UserGroupRequestDto>(
            elements,
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );

        logger.LogInformation("Returning {Count} join requests for group {GroupId}.", result.TotalItems, request.GroupId);

        return ResultT<PagedResult<UserGroupRequestDto>>.Success(pagedResult);
    }
}
