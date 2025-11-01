using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;
using Rex.Application.DTOs.File;
using Rex.Application.DTOs.Reply;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Comments.Queries.GetCommentsByPostId;

public class GetCommentsByPostIdQueryHandler(
    ILogger<GetCommentsByPostIdQueryHandler> logger,
    ICommentRepository commentRepository,
    IFileRepository fileRepository,
    IDistributedCache cache
) : IQueryHandler<GetCommentsByPostIdQuery, PagedResult<CommentDetailsDto>>
{
    public async Task<ResultT<PagedResult<CommentDetailsDto>>> Handle(GetCommentsByPostIdQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetCommentsByPostIdQuery for PostId: {PostId}, Page: {Page}, Size: {Size}",
            request.PostId, request.PageNumber, request.PageSize);

        var version = await cache.GetVersionAsync("comments", request.PostId, cancellationToken);

        var comments = await cache.GetOrCreateAsync(
            $"Get:Comments:By:Post:{request.PostId}:{request.PageNumber}:{request.PageSize}:version:{version}:",
            async () =>
            {
                logger.LogInformation("Cache miss: fetching comments from repository for PostId: {PostId}", request.PostId);
                return await commentRepository.GetCommentsPaginatedByPostIdAsync(
                    request.PostId, request.PageNumber, request.PageSize, cancellationToken);
            },
            logger,
            cancellationToken: cancellationToken
        );

        if (!comments.Items.Any())
        {
            logger.LogWarning("No comments found for PostId: {PostId}", request.PostId);
            return ResultT<PagedResult<CommentDetailsDto>>.Success(
                new PagedResult<CommentDetailsDto>([], comments.TotalItems, comments.ActualPage, comments.TotalPages));
        }

        var files = await cache.GetOrCreateAsync(
            $"Get:Comment:Files:Replies:{request.PostId}:{request.PageNumber}:{request.PageSize}",
            async () => await fileRepository.GetFilesByTargetIdsAsync(comments.Items.Select(c => c.Id), TargetType.Comment, cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );
        
        logger.LogInformation("Mapping {Count} comments to DTOs for PostId: {PostId}", comments.Items.Count(), request.PostId);

        var elements = comments.Items.Select(c =>
        {
            var commentFiles = files.Where(f => f.EntityFiles.Any(e => e.TargetId == c.Id))
                .Select(f => new FileDetailDto(f.Id, f.Url, f.Type));

            return new CommentDetailsDto(
                c.Id,
                c.PostId,
                c.Description,
                c.IsPinned,
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

        var result = new PagedResult<CommentDetailsDto>(
            elements,
            comments.TotalItems,
            comments.ActualPage,
            comments.TotalPages
        );

        logger.LogInformation("Successfully handled GetCommentsByPostIdQuery for PostId: {PostId}", request.PostId);

        return ResultT<PagedResult<CommentDetailsDto>>.Success(result);
    }
}
