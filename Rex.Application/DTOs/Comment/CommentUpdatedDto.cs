namespace Rex.Application.DTOs.Comment;

public record CommentUpdatedDto(
    Guid CommentId,
    string Description,
    bool IsPinned,
    bool Edited
    );