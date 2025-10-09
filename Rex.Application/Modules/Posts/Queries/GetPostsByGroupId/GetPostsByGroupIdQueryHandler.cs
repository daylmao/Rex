using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Posts.Queries.GetPostsByGroupId;

public class GetPostsByGroupIdQueryHandler(
    ILogger<GetPostsByGroupIdQueryHandler> logger,
    IPostRepository postRepository,
    IGroupRepository groupRepository,
    IFileRepository fileRepository,
    ICommentRepository commentRepository,
    IReactionRepository reactionRepository,
    IDistributedCache cache
) : IQueryHandler<GetPostsByGroupIdQuery, PagedResult<PostDetailsDto>>
{
    public async Task<ResultT<PagedResult<PostDetailsDto>>> Handle(GetPostsByGroupIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request for fetching posts by GroupId.");
            return ResultT<PagedResult<PostDetailsDto>>.Failure(Error.Failure("400",
                "Oops! We couldn't process your request. Please provide the necessary details."));
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("No group found with the provided GroupId: {GroupId}.", request.GroupId);
            return ResultT<PagedResult<PostDetailsDto>>.Failure(Error.Failure("404",
                "We couldn't find the group you selected."));
        }

        var posts = await cache.GetOrCreateAsync(
            $"posts:group:{request.GroupId}:page:{request.PageNumber}:size:{request.PageSize}",
            async () => await postRepository.GetPostsByGroupIdAsync(
                request.GroupId, request.PageNumber, request.PageSize, cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );

        if (!posts.Items.Any())
        {
            logger.LogInformation("No posts found for group ID {GroupId}", request.GroupId);
            return ResultT<PagedResult<PostDetailsDto>>.Success(
                new PagedResult<PostDetailsDto>([], posts.TotalItems, posts.ActualPage, posts.TotalPages)
            );
        }

        var postIds = posts.Items.Select(p => p.Id).ToList();

        var commentCountsTask = commentRepository.GetCommentsCountByPostIdsAsync(postIds, cancellationToken);
        var likeCountsTask =
            reactionRepository.GetLikesCountByPostIdsAsync(postIds, TargetType.Post, cancellationToken);
        var filesTask = fileRepository.GetFilesByTargetIdsAsync(postIds, TargetType.Post, cancellationToken);

        await Task.WhenAll(commentCountsTask, likeCountsTask, filesTask);

        var commentCounts = await commentCountsTask;
        var likeCounts = await likeCountsTask;
        var files = await filesTask;

        var filesByPostId = files
            .SelectMany(f => f.EntityFiles, (file, entityFile) =>
                new
                {
                    entityFile.TargetId,
                    File = new FileDetailDto(file.Url, file.Type)
                })
            .GroupBy(c => c.TargetId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.File).ToList());

        var elements = posts.Items.Select(p =>
        {
            commentCounts.TryGetValue(p.Id, out var commentCount);
            likeCounts.TryGetValue(p.Id, out var likeCount);
            var fileUrls = filesByPostId.GetValueOrDefault(p.Id) ?? [];

            return new PostDetailsDto(
                $"{p.User.FirstName} {p.User.LastName}",
                p.Title,
                p.Description,
                p.User.ProfilePhoto,
                likeCount,
                commentCount,
                p.CreatedAt,
                fileUrls
            );
        }).ToList();

        var result = new PagedResult<PostDetailsDto>(elements, posts.TotalItems, posts.ActualPage, posts.TotalPages);

        logger.LogInformation("Successfully retrieved {Count} posts for group {GroupId}",
            elements.Count, request.GroupId);

        return ResultT<PagedResult<PostDetailsDto>>.Success(result);
    }
}