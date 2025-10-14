using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Comments.Queries.GetCommentsByPostId;

public record GetCommentsByPostIdQuery(
    Guid PostId,
    int PageNumber,
    int PageSize
    ): IQuery<PagedResult<CommentDetailsDto>>;