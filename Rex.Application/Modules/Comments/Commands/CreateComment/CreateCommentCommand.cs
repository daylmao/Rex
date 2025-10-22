using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;

namespace Rex.Application.Modules.Comments.Commands.CreateComment;

public record CreateCommentCommand(
    Guid PostId,
    Guid UserId,
    string Description,
    IEnumerable<IFormFile>? Files = null
    ):ICommand<CommentDto>;