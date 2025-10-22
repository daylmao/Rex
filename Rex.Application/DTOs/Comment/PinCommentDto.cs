namespace Rex.Application.DTOs.Comment;

public record PinCommentDto(
    Guid CommentId,
    Guid PostId,
    bool Pin
    );