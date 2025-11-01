using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.File;
using Rex.Application.DTOs.Reply;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Comments.Queries.GetCommentReplies;

public class GetCommentRepliesQueryHandler(
    ILogger<GetCommentRepliesQueryHandler> logger,
    ICommentRepository commentRepository,
    IFileRepository fileRepository,
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

        var version = await cache.GetVersionAsync("comments", request.ParentCommentId, cancellationToken);

        var comments = await cache.GetOrCreateAsync(
            $"Get:Comment:Replies:{request.ParentCommentId}:{request.PageNumber}:{request.PageSize}:version:{version}:",
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

        var files = await cache.GetOrCreateAsync(
            $"Get:Comment:Files:Replies:{request.ParentCommentId}:{request.PageNumber}:{request.PageSize}",
            async () => await fileRepository.GetFilesByTargetIdsAsync(
                comments.Items.Select(c => c.Id), 
                TargetType.Comment, 
                cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );

        var elements = comments.Items.Select(c =>
        {
            var commentFiles = files.Where(f => f.EntityFiles.Any(e => e.TargetId == c.Id))
                .Select(f => new FileDetailDto(f.Id, f.Url, f.Type));

            return new ReplyDto(
                c.Id,
                c.ParentCommentId!.Value, 
                c.Description,
                c.Edited,
                c.Replies.Any(),
                new UserCommentDetailsDto(
                    c.UserId,
                    c.User.FirstName,
                    c.User.LastName,
                    c.User.ProfilePhoto),
                c.CreatedAt,
                commentFiles
            );
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
