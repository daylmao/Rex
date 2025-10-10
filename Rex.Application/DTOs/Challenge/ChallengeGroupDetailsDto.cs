namespace Rex.Application.DTOs;

public record ChallengeGroupDetailsDto(
    Guid ChallengeId,
    string Title,
    string Description,
    string Status,
    string CoverPhoto,
    TimeSpan Duration,
    List<string> UserProfilePhotos,
    int TotalParticipants
        );