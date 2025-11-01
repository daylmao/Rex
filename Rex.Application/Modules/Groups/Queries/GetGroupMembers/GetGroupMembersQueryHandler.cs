using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Groups.Queries.GetGroupMembers;

public class GetGroupMembersQueryHandler(
    ILogger<GetGroupMembersQueryHandler> logger,
    IGroupRepository groupRepository,
    IUserGroupRepository userGroupRepository,
    IDistributedCache cache
) : IQueryHandler<GetGroupMembersQuery, PagedResult<UserGroupDetailsDto>>
{
    public async Task<ResultT<PagedResult<UserGroupDetailsDto>>> Handle(GetGroupMembersQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("GetGroupMembersQueryHandler: Request is null");
            return ResultT<PagedResult<UserGroupDetailsDto>>.Failure(
                Error.Failure("400", "Oops! No data was provided to fetch group members."));
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("GetGroupMembersQueryHandler: Group with Id {GroupId} not found", request.GroupId);
            return ResultT<PagedResult<UserGroupDetailsDto>>.Failure(
                Error.NotFound("404", "We couldn't find the group you're looking for."));
        }

        var searchTerm = request.SearchTerm ?? "";
        var roleFilter = request.RoleFilter?.ToString() ?? "all";

        var version = await cache.GetVersionAsync("group-members", request.GroupId, cancellationToken);
        var cacheKey = $"group-members:group:{request.GroupId}:role:{roleFilter}:search:{searchTerm}:page:{request.PageNumber}:size:{request.PageSize}:version:{version}";
        
        var result = await cache.GetOrCreateAsync(
            cacheKey,
            async () => await userGroupRepository.GetMembersAsync(request.GroupId, request.RoleFilter, request.SearchTerm, request.PageNumber, request.PageSize, cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );


        if (!result.Items.Any())
        {
            logger.LogInformation("No members found for group {GroupId}", request.GroupId);
            return ResultT<PagedResult<UserGroupDetailsDto>>.Success(
                new PagedResult<UserGroupDetailsDto>([], result.TotalItems, result.ActualPage, result.TotalPages)
            );
        }

        var elements = result.Items
            .Select(ug => new UserGroupDetailsDto(
                ug.UserId,
                ug.User.FirstName,
                ug.User.LastName,
                ug.GroupRole.Role,
                ug.User.ProfilePhoto
            ))
            .ToList();

        var pagedResult = new PagedResult<UserGroupDetailsDto>(
            elements,
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );

        logger.LogInformation("Successfully retrieved {Count} members for group {GroupId}", elements.Count,
            request.GroupId);
        return ResultT<PagedResult<UserGroupDetailsDto>>.Success(pagedResult);
    }
}