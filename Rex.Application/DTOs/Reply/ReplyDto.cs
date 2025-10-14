using Rex.Application.DTOs.User;

namespace Rex.Application.DTOs.Reply;

public record ReplyDto(
    Guid ReplyId,
    Guid ParentCommentId,
    string Description,
    bool Edited,
    bool HasReplies,
    UserCommentDetailsDto User,
    DateTime CreatedAt
);
