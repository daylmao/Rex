using Microsoft.AspNetCore.SignalR;
using Rex.Application.DTOs.Notification;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Infrastructure.Shared.Services.SignalR.Hubs;
using Rex.Models;

namespace Rex.Infrastructure.Shared.Services.SignalR;

public class ChallengeNotifier(
    IHubContext<AppHub, IAppHub> hubContext,
    INotificationRepository notificationRepository
    ): IChallengeNotifier
{
    public async Task SendChallengeNotification(Notification notificationChallenge, CancellationToken cancellationToken)
    {
        await notificationRepository.CreateAsync(notificationChallenge, cancellationToken);

        var notification = new NotificationDto(
            Title: notificationChallenge.Title,
            Description: notificationChallenge.Description,
            UserId: notificationChallenge.UserId,
            RecipientId: notificationChallenge.RecipientId, 
            CreatedAt: notificationChallenge.CreatedAt,
            IsRead: notificationChallenge.Read
        );

        await hubContext.Clients.Group(notification.RecipientId.ToString())
            .ReceiveFriendRequestNotification(notification);
    }
}