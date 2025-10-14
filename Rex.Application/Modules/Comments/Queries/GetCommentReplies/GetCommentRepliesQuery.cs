using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Reply;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Comments.Queries.GetCommentReplies;

public record GetCommentRepliesQuery(
    Guid PostId,
    Guid ParentCommentId,
    int PageNumber,
    int PageSize
    ): IQuery<PagedResult<ReplyDto>>;