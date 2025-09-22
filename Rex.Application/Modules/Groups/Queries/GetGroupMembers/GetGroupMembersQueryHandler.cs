using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Groups.Queries.GetGroupMembers;

public class GetGroupMembersQueryHandler(
    ILogger<GetGroupMembersQueryHandler> logger,
    IGroupRepository groupRepository,
    IUserGroupRepository userGroupRepository,
    IDistributedCache cache
    ): IQueryHandler<GetGroupMembersQuery, PagedResult<UserGroupDetailsDto>>
{
    public async Task<ResultT<PagedResult<UserGroupDetailsDto>>> Handle(GetGroupMembersQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("GetGroupMembersQueryHandler: Request is null");
            return ResultT<PagedResult<UserGroupDetailsDto>>.Failure(Error.Failure("400", "Request is null"));
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("GetGroupMembersQueryHandler: Group with Id {GroupId} not found", request.GroupId);
            return ResultT<PagedResult<UserGroupDetailsDto>>.Failure(Error.NotFound("404", "Group not found"));
        }
        
        var result = await cache.GetOrCreateAsync(
            $"GetGroupMembers-{request.GroupId.ToString()}-{request.PageNumber}-{request.PageSize}-{request.SearchTerm}-{request.RoleFilter}",
            async () => await userGroupRepository.GetMembersAsync(request.GroupId, request.RoleFilter, request.SearchTerm,
                request.PageNumber, request.PageSize, cancellationToken),
                logger, cancellationToken: cancellationToken
            );
        
        var elements = result.Items
            .Select(ug => new UserGroupDetailsDto(
                ug.UserId,
                ug.User.FirstName,
                ug.User.LastName,
                ug.GroupRole.Role,
                ug.User.ProfilePhoto
            ));

        if (!elements.Any())
        {
            logger.LogWarning("GetGroupMembersQueryHandler: No members found for group with Id {GroupId}", request.GroupId);
            return ResultT<PagedResult<UserGroupDetailsDto>>.Failure(Error.NotFound("404", "No members found"));
        }
        var pagedResult = new PagedResult<UserGroupDetailsDto>(
            elements,
            result.TotalItems,
            request.PageNumber,
            request.PageSize
        );
        
        return ResultT<PagedResult<UserGroupDetailsDto>>.Success(pagedResult);
    }
}