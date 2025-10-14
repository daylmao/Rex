using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Reply;

namespace Rex.Application.Modules.Comments.Commands.CreateCommentReply;

public record CreateCommentReplyCommand(
    Guid ParentCommentId,
    Guid PostId,
    Guid UserId,
    string Description
) : ICommand<ReplyDto>;
