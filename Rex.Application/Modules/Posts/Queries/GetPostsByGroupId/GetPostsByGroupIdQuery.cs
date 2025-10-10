using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Post;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Posts.Queries.GetPostsByGroupId;

public record GetPostsByGroupIdQuery(
    Guid GroupId,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<PostDetailsDto>>;
