using Rex.Application.DTOs.Reply;
using Rex.Application.DTOs.User;

namespace Rex.Application.DTOs.Comment;

public record CommentDetailsDto(
    Guid CommentId,
    Guid PostId,
    string Description,
    bool IsPinned,
    bool Edited,
    bool HasReplies,
    UserCommentDetailsDto User,
    DateTime CreatedAt
);
