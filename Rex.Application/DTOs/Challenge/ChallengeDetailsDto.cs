namespace Rex.Application.DTOs.Challenge;

public record ChallengeDetailsDto(
    Guid ChallengeId,
    string Title,
    string Description,
    string Status,
    string CoverPhoto,
    TimeSpan Duration
    );