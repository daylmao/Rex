namespace Rex.Application.DTOs;

public record NotificationDto(
    string Title,
    string Description,
    Guid UserId,
    Guid RecipientId,
    DateTime CreatedAt,
    bool IsRead = false
);