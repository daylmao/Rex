namespace Rex.Application.DTOs;

public record ChatLastMessageDto(
    Guid Id,
    string? Name,
    string Type,
    string ProfilePicture,
    LastMessageDto? LastMessage
    );