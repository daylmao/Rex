using Microsoft.AspNetCore.Http;

namespace Rex.Application.DTOs.Comment;

public record CreateCommentReplyDto(
    Guid ParentCommentId,
    Guid PostId,
    string Description,
    IEnumerable<IFormFile>? Files = null 
    );