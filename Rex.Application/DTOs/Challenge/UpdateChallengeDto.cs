using Microsoft.AspNetCore.Http;
using Rex.Enum;

namespace Rex.Application.DTOs.Challenge;

public record UpdateChallengeDto(
    string Title,
    string Description,
    TimeSpan Duration,
    ChallengeStatus Status
    );