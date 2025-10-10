namespace Rex.Application.DTOs.Notification;

public record NotificationDto(
    string Title,
    string Description,
    Guid UserId,
    Guid RecipientId,
    DateTime CreatedAt,
    bool IsRead = false
);