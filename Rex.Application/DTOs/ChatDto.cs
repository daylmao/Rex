namespace Rex.Application.DTOs;

public record ChatDto(
    Guid Id,
    string? Name,
    string Type,
    string ProfilePicture
    );