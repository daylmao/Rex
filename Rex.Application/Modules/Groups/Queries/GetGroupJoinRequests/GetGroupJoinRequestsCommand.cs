using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.User;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Groups.Queries.GetGroupJoinRequests;

public record GetGroupJoinRequestsCommand(
    Guid GroupId,
    int PageNumber,
    int PageSize,
    string? SearchTerm
    ): IQuery<PagedResult<UserGroupRequestDto>>;