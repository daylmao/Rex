namespace Rex.Application.DTOs.Message;

public record LastMessageDto(
    Guid MessageId,
    string Description,
    DateTime CreatedAt,
    Guid SenderId,
    string SenderName,
    string SenderProfilePhoto
);
