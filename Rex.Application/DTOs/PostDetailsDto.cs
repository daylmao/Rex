namespace Rex.Application.DTOs;

public record PostDetailsDto(
    string Name,
    string Title,
    string Content,
    string? ProfileImage,
    int? LikesCount,
    int? CommentsCount,
    DateTime CreatedAt,
    IEnumerable<FileDetailDto>? Files
);