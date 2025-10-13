namespace Rex.Application.DTOs.Challenge;

public record ChallengeUserDetailsDto(
    Guid ChallengeId,
    string ProfilePhoto,
    string CoverPhoto,
    string Title,
    string Description,
    string Status,
    TimeSpan Duration,
    string GroupTitle,
    List<string> UserProfilePhotos,
    int TotalParticipants
    );