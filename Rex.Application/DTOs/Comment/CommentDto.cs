using Microsoft.AspNetCore.Http;

namespace Rex.Application.DTOs.Comment;

public record CommentDto(
    Guid CommentId,
    Guid PostId,
    Guid UserId,
    string Description,
    bool IsPinned,
    bool Edited,
    IEnumerable<Models.File>? Files = null
        );