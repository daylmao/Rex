using Rex.Application.DTOs.File;

namespace Rex.Application.DTOs.Post;

public record PostDetailsDto(
    Guid PostId,
    string Name,
    string Title,
    string Content,
    string? ProfileImage,
    int? LikesCount,
    int? CommentsCount,
    DateTime CreatedAt,
    bool? ChallengeCompleted,
    IEnumerable<FileDetailDto>? Files
);