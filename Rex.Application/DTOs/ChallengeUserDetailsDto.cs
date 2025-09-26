namespace Rex.Application.DTOs;

public record ChallengeUserDetailsDto(
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