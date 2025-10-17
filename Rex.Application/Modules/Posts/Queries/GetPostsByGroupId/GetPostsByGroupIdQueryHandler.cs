using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.File;
using Rex.Application.DTOs.Post;
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
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("No group found with GroupId: {GroupId}.", request.GroupId);
            return ResultT<PagedResult<PostDetailsDto>>.Failure(Error.NotFound("404",
                "We couldn't find the group you're looking for."));
        }

        var posts = await cache.GetOrCreateAsync(
            $"posts:group:{request.GroupId}:user:{request.UserId}:page:{request.PageNumber}:size:{request.PageSize}",
            async () => await postRepository.GetPostsByGroupIdAsync(
                request.GroupId, request.PageNumber, request.PageSize, cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );

        if (!posts.Items.Any())
        {
            logger.LogInformation("No posts found for group {GroupId}.", request.GroupId);
            return ResultT<PagedResult<PostDetailsDto>>.Success(
                new PagedResult<PostDetailsDto>([], posts.TotalItems, posts.ActualPage, posts.TotalPages)
            );
        }

        var postIds = posts.Items.Select(p => p.Id).ToList();

        var commentCounts = await commentRepository.GetCommentsCountByPostIdsAsync(postIds, cancellationToken);
        var likeCounts = await reactionRepository.GetLikesCountByPostIdsAsync(postIds, TargetType.Post, cancellationToken);
        var files = await fileRepository.GetFilesByTargetIdsAsync(postIds, TargetType.Post, cancellationToken);
        var userLikes = await reactionRepository.GetUserLikesForTargetsAsync(
            request.UserId, postIds, ReactionTargetType.Post, cancellationToken);

        var filesByPostId = files
            .SelectMany(f => f.EntityFiles, (file, entityFile) =>
                new
                {
                    entityFile.TargetId,
                    File = new FileDetailDto(file.Id,file.Url, file.Type)
                })
            .GroupBy(c => c.TargetId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.File).ToList());

        var elements = posts.Items.Select(p =>
        {
            commentCounts.TryGetValue(p.Id, out var commentCount);
            likeCounts.TryGetValue(p.Id, out var likeCount);
            var hasUserLiked = userLikes.Contains(p.Id);
            var fileUrls = filesByPostId.GetValueOrDefault(p.Id) ?? [];

            var hasCompletedChallenge = p.ChallengeId.HasValue &&
                                        p.User.UserChallenges.Any(uc =>
                                            uc.ChallengeId == p.ChallengeId &&
                                            uc.Status == UserChallengeStatus.Completed.ToString()
                                        );

            return new PostDetailsDto(
                p.Id,
                p.UserId,
                p.GroupId,
                $"{p.User.FirstName} {p.User.LastName}",
                p.Title,
                p.Description,
                p.User.ProfilePhoto,
                likeCount,
                commentCount,
                hasUserLiked,
                p.CreatedAt,
                hasCompletedChallenge,
                fileUrls
            );
        }).ToList();

        var result = new PagedResult<PostDetailsDto>(elements, posts.TotalItems, posts.ActualPage, posts.TotalPages);

        logger.LogInformation("Successfully retrieved {Count} posts for group {GroupId}.",
            elements.Count, request.GroupId);

        return ResultT<PagedResult<PostDetailsDto>>.Success(result);
    }
}
