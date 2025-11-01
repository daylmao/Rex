namespace Rex.Application.DTOs.Notification;

public record NotificationDto(
    Guid Id,
    string Title,
    string Description,
    Guid UserId,
    string RecipientType,
    Guid RecipientId,
    string MetadataJson,
    DateTime CreatedAt,
    bool IsRead = false
);