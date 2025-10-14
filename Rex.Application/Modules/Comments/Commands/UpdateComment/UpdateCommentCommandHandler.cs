using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Comments.Commands.UpdateComment;

public class UpdateCommentCommandHandler(
    ILogger<UpdateCommentCommandHandler> logger,
    ICommentRepository commentRepository
) : ICommandHandler<UpdateCommentCommand, CommentUpdatedDto>
{
    public async Task<ResultT<CommentUpdatedDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Attempting to update comment with ID: {CommentId}. New description: {Description}",
            request.CommentId, request.Description
        );

        var comment = await commentRepository.GetByIdAsync(request.CommentId, cancellationToken);

        if (comment is null)
        {
            logger.LogWarning(
                "Comment with ID: {CommentId} not found. Update operation failed.",
                request.CommentId
            );

            return ResultT<CommentUpdatedDto>.Failure(
                Error.NotFound(
                    "404",
                    "We couldn't find the comment you were trying to update. It may have been deleted or never existed."
                )
            );
        }

        comment.Description = request.Description;
        await commentRepository.UpdateAsync(comment, cancellationToken);

        var dto = new CommentUpdatedDto(
            comment.Id,
            comment.Description,
            comment.IsPinned,
            comment.Edited
        );

        logger.LogInformation(
            "Comment with ID: {CommentId} successfully updated. Description changed to: {Description}",
            comment.Id, comment.Description
        );

        return ResultT<CommentUpdatedDto>.Success(dto);
    }
}