namespace Rex.Application.DTOs;

public record LastMessageDto(
    string Description,
    DateTime CreatedAt,
    Guid SenderId,
    string SenderName,
    string SenderProfilePhoto
);
