using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;

namespace Rex.Application.Modules.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(
    Guid CommentId,
    string Description,
    IEnumerable<IFormFile>? NewFiles = null,
    IEnumerable<Guid>? FilesToDelete = null
        ): ICommand<CommentUpdatedDto>;