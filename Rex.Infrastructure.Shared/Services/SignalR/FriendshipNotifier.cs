using Microsoft.AspNetCore.SignalR;
using Rex.Application.DTOs.Notification;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Infrastructure.Shared.Services.SignalR.Hubs;
using Rex.Models;

namespace Rex.Infrastructure.Shared.Services.SignalR;

public class FriendshipNotifier(
    IHubContext<AppHub, IAppHub> hubContext,
    INotificationRepository notificationRepository
    ): IFriendshipNotifier
{
    public async Task SendFriendRequestNotification(Notification notification, CancellationToken cancellationToken)
    {
        await notificationRepository.CreateAsync(notification, cancellationToken);

        var notificationDto = new NotificationDto(
            Title: notification.Title,
            Description: notification.Description,
            UserId: notification.UserId,
            RecipientId: notification.RecipientId, 
            CreatedAt: notification.CreatedAt,
            IsRead: notification.Read
        );

        await hubContext.Clients.Group(notification.RecipientId.ToString())
            .ReceiveFriendRequestNotification(notificationDto);
    }
}