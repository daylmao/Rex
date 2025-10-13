using Rex.Application.DTOs.File;

namespace Rex.Application.DTOs.Post;

public record PostDetailsDto(
    Guid PostId,
    Guid AuthorId,
    Guid GroupId,
    string AuthorName,
    string Title,
    string Description,
    string ProfilePhoto,
    int LikeCount,
    int CommentCount,
    bool HasLiked,             
    DateTime CreatedAt,
    bool HasCompletedChallenge,
    List<FileDetailDto> Files
);