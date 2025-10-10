using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.User;
using Rex.Application.Pagination;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Queries.GetGroupMembers;

public record GetGroupMembersQuery(
    Guid GroupId,
    int PageNumber,
    int PageSize,
    string? SearchTerm,
    GroupRole? RoleFilter
    ): IQuery<PagedResult<UserGroupDetailsDto>>;