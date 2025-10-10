namespace Rex.Application.DTOs.Challenge;

public record ChatDto(
    Guid Id,
    string? Name,
    string Type,
    string ProfilePicture
    );