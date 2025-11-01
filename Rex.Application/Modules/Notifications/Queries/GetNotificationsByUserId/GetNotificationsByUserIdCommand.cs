using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Notification;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Notifications.Queries.GetNotificationsByUserId;

public record GetNotificationsByUserIdCommand(
    Guid UserId,
    int PageNumber,
    int PageSize
    ): IQuery<PagedResult<NotificationDto>>;