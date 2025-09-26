using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Groups.Queries.GetGroupsByUserId;

public record GetGroupsByUserIdQuery(
    Guid UserId,
    int pageNumber,
    int pageSize
    ): IQuery<PagedResult<GroupDetailsDto>>;