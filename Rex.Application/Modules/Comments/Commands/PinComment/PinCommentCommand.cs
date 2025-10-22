using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;

namespace Rex.Application.Modules.Comments.Commands.PinComment;

public record PinCommentCommand(
    Guid CommentId,
    Guid UserId,
    Guid PostId,
    bool Pin
    ):ICommand<CommentUpdatedDto>;