using Rex.Application.DTOs.Message;

namespace Rex.Application.DTOs.Challenge;

public record ChatLastMessageDto(
    Guid Id,
    string? Name,
    string Type,
    string ProfilePicture,
    LastMessageDto? LastMessage
    );