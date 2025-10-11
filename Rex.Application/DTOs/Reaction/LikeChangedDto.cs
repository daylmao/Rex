namespace Rex.Application.DTOs.Reaction;

public record LikeChangedDto(
    Guid PostId,
    int TotalLikes,
    Guid UserId,
    bool Liked
);
