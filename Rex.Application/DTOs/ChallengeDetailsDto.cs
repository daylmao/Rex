namespace Rex.Application.DTOs;

public record ChallengeDetailsDto(
    Guid ChallengeId,
    string Title,
    string Description,
    string Status,
    TimeSpan Duration
    );