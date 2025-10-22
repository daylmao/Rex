using Microsoft.AspNetCore.Http;

namespace Rex.Application.DTOs.Comment;

public record CreateCommentDto(
    Guid PostId,
    string Description,
    IEnumerable<IFormFile>? Files = null
    );