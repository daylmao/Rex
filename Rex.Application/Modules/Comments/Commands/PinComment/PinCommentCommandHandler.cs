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
    IUserRepository userRepository
    ): ICommandHandler<PinCommentCommand, CommentUpdatedDto>
{
    public async Task<ResultT<CommentUpdatedDto>> Handle(PinCommentCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("We couldn't find the user with ID {UserId}.", request.UserId);
            return ResultT<CommentUpdatedDto>.Failure(Error.NotFound("404", "We couldn't find a user with that ID."));
        }

        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.UserId, cancellationToken);
        if (!accountConfirmed)
        {
            logger.LogWarning("User with ID {UserId} tried to create a group but the account is not confirmed.", request.UserId);
            return ResultT<CommentUpdatedDto>.Failure(Error.Failure("403", "You need to confirm your account before creating a group."));
        }
        
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
        {
            logger.LogWarning("We couldn't find the post with ID {PostId}.", request.PostId);
            return ResultT<CommentUpdatedDto>.Failure(Error.NotFound("404", "The post you're looking for doesn't exist."));
        }

        var comment = await commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment is null)
        {
            logger.LogWarning("We couldn't find the comment with ID {CommentId}.", request.CommentId);
            return ResultT<CommentUpdatedDto>.Failure(Error.NotFound("404", "The comment doesn't exist."));
        }

        if (post.UserId != request.UserId)
        {
            logger.LogWarning("User {UserId} attempted to pin a comment on a post they don't own. PostId: {PostId}", request.UserId, request.PostId);
            return ResultT<CommentUpdatedDto>.Failure(Error.Failure("401", "You don't have permission to pin comments on this post."));
        }

        comment.IsPinned = request.Pin;
        await commentRepository.UpdateAsync(comment, cancellationToken);
    
        var dto = new CommentUpdatedDto(
            comment.Id,
            comment.Description,
            comment.IsPinned,
            comment.Edited
        );
    
        logger.LogInformation("Comment with ID {CommentId} has been successfully pinned.", comment.Id);
        return ResultT<CommentUpdatedDto>.Success(dto);
    }
}
