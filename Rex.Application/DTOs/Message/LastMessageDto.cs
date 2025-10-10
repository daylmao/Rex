namespace Rex.Application.DTOs;

public record LastMessageDto(
    Guid MessageId,
    string Description,
    DateTime CreatedAt,
    Guid SenderId,
    string SenderName,
    string SenderProfilePhoto
);
