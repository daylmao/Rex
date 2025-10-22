using Microsoft.AspNetCore.Http;

namespace Rex.Application.DTOs.Challenge;

public record CreateChallengeDto(
    Guid GroupId,
    string Title,
    string Description,
    TimeSpan Duration,
    IFormFile CoverPhoto
    );