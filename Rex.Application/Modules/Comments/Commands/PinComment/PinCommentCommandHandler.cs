using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Comments.Commands.PinComment;

public class PinCommentCommandHandler(
    ILogger<PinCommentCommandHandler> logger,
    ICommentRepository commentRepository,
    IPostRepository postRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : ICommandHandler<PinCommentCommand, CommentUpdatedDto>
{
    public async Task<ResultT<CommentUpdatedDto>> Handle(PinCommentCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("We couldn't find the user with ID {UserId}.", request.UserId);
            return ResultT<CommentUpdatedDto>.Failure(Error.NotFound("404", "Oops! We couldn’t find that user."));
        }

        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.UserId, cancellationToken);
        if (!accountConfirmed)
        {
            logger.LogWarning("User {UserId} tried to create a group but the account is not confirmed.",
                request.UserId);
            return ResultT<CommentUpdatedDto>.Failure(Error.Failure("403", "You need to confirm your account first."));
        }

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
        {
            logger.LogWarning("We couldn't find the post with ID {PostId}.", request.PostId);
            return ResultT<CommentUpdatedDto>.Failure(Error.NotFound("404",
                "The post you are looking for doesn't exist."));
        }

        var comment = await commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment is null)
        {
            logger.LogWarning("We couldn't find the comment with ID {CommentId}.", request.CommentId);
            return ResultT<CommentUpdatedDto>.Failure(Error.NotFound("404", "The comment doesn't exist."));
        }

        if (post.UserId != request.UserId)
        {
            logger.LogWarning("User {UserId} tried to pin a comment on a post they don't own. PostId: {PostId}",
                request.UserId, request.PostId);
            return ResultT<CommentUpdatedDto>.Failure(Error.Failure("401",
                "You can't pin comments on a post you don't own."));
        }

        if (request.Pin)
        {
            var alreadyPinned = await commentRepository.CommentAlreadyPinned(request.CommentId, cancellationToken);
            if (alreadyPinned)
            {
                logger.LogWarning("Comment {CommentId} is already pinned.", request.CommentId);
                return ResultT<CommentUpdatedDto>.Failure(Error.Failure("400", "This comment is already pinned."));
            }

            var anotherPinned = await commentRepository.AnotherCommentIsPinned(request.PostId, cancellationToken);
            if (anotherPinned)
            {
                logger.LogWarning("Another comment is already pinned on post {PostId}.", request.PostId);
                return ResultT<CommentUpdatedDto>.Failure(Error.Failure("400",
                    "There is already a pinned comment on this post."));
            }
        }

        comment.IsPinned = request.Pin;
        await commentRepository.UpdateAsync(comment, cancellationToken);
        
        await cache.IncrementVersionAsync("comments", comment.PostId, logger, cancellationToken);
        logger.LogInformation("Cache invalidated for comments of PostId: {PostId}", comment.PostId);

        var dto = new CommentUpdatedDto(
            comment.Id,
            comment.Description,
            comment.IsPinned,
            comment.Edited
        );

        logger.LogInformation("Comment with ID {CommentId} has been successfully {Action}.", comment.Id,
            request.Pin ? "pinned" : "unpinned");
        return ResultT<CommentUpdatedDto>.Success(dto);
    }
}