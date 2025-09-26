using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Groups.Queries.GetGroupsPaginated;

public record GetGroupsPaginatedQuery(
    Guid UserId,
    int pageNumber,
    int pageSize
    ): IQuery<PagedResult<GroupDetailsDto>>;