using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Posts.Queries.GetPostsByGroupId;

public record GetPostsByGroupIdQuery(
    Guid GroupId,
    int PageNumber,
    int PageSize,
    Guid? ChallengeId = null
) : IQuery<PagedResult<PostDetailsDto>>;
