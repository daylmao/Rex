using Microsoft.AspNetCore.SignalR;
using Rex.Application.DTOs.Notification;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Infrastructure.Shared.Services.SignalR.Hubs;
using Rex.Models;

namespace Rex.Infrastructure.Shared.Services.SignalR;

public class InactiveUserNotifier(
    IHubContext<AppHub, IAppHub> hubContext,
    INotificationRepository notificationRepository
) : IInactiveUserNotifier
{
    public async Task SendWarnNotification(Notification notification, CancellationToken cancellationToken)
    {
        await notificationRepository.CreateAsync(notification, cancellationToken);

        var notificationDto = new NotificationDto(
            Id: notification.Id,
            Title: notification.Title,
            Description: notification.Description,
            UserId: notification.UserId,
            RecipientType: notification.RecipientType,
            RecipientId: notification.RecipientId,
            MetadataJson: notification.MetadataJson,
            CreatedAt: notification.CreatedAt,
            IsRead: notification.Read
        );

        await hubContext.Clients.User(notificationDto.RecipientId.ToString())
            .ReceiveWarnNotification(notificationDto);
    }

    public async Task SendBanNotification(Notification notification, CancellationToken cancellationToken)
    {
        await notificationRepository.CreateAsync(notification, cancellationToken);

        var notificationDto = new NotificationDto(
            Id: notification.Id,
            Title: notification.Title,
            Description: notification.Description,
            UserId: notification.UserId,
            RecipientType: notification.RecipientType,
            RecipientId: notification.RecipientId,
            MetadataJson: notification.MetadataJson,
            CreatedAt: notification.CreatedAt,
            IsRead: notification.Read
        );

        await hubContext.Clients.User(notificationDto.RecipientId.ToString())
            .ReceiveRemoveNotification(notificationDto);
    }
}