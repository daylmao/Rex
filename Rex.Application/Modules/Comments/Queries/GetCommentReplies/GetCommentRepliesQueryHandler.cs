using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;
using Rex.Application.DTOs.Reply;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Comments.Queries.GetCommentReplies;

public class GetCommentRepliesQueryHandler(
    ILogger<GetCommentRepliesQueryHandler> logger,
    ICommentRepository commentRepository,
    IDistributedCache cache
) : IQueryHandler<GetCommentRepliesQuery, PagedResult<ReplyDto>>
{
    public async Task<ResultT<PagedResult<ReplyDto>>> Handle(GetCommentRepliesQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Fetching replies for ParentCommentId: {ParentCommentId}, PostId: {PostId}, Page: {Page}, PageSize: {PageSize}",
            request.ParentCommentId, request.PostId, request.PageNumber, request.PageSize
        );

        var comments = await cache.GetOrCreateAsync(
            $"Get:Comment:Replies:{request.ParentCommentId}:{request.PageNumber}:{request.PageSize}",
            async () => await commentRepository.GetCommentsRepliedPaginatedByParentCommentIdAsync(
                request.PostId,
                request.PageNumber,
                request.PageSize,
                request.ParentCommentId,
                cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );

        if (!comments.Items.Any())
        {
            logger.LogWarning(
                "No replies found for ParentCommentId: {ParentCommentId} in PostId: {PostId}.",
                request.ParentCommentId, request.PostId
            );

            return ResultT<PagedResult<ReplyDto>>.Failure(Error.NotFound(
                "404", 
                "We couldn't find any replies for this comment. It may not exist or hasn't received any replies yet."
            ));
        }

        var elements = comments.Items.Select(c =>
        {
            return new ReplyDto(
                c.Id,
                c.PostId,
                c.Description,
                c.Edited,
                c.Replies.Any(),
                new UserCommentDetailsDto(
                    c.UserId,
                    c.User.FirstName,
                    c.User.LastName,
                    c.User.ProfilePhoto),
                c.CreatedAt);
        });

        logger.LogInformation(
            "Successfully retrieved {Count} replies for ParentCommentId: {ParentCommentId}",
            comments.Items.Count(), request.ParentCommentId
        );

        var result = new PagedResult<ReplyDto>(
            elements,
            comments.TotalItems,
            comments.ActualPage,
            comments.TotalPages
        );
        
        return ResultT<PagedResult<ReplyDto>>.Success(result);
    }
}
