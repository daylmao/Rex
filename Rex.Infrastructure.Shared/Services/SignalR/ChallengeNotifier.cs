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
) : IChallengeNotifier
{
    public async Task SendChallengeNotification(Notification notificationChallenge, CancellationToken cancellationToken)
    {
        await notificationRepository.CreateAsync(notificationChallenge, cancellationToken);

        var notificationDto = new NotificationDto(
            Id: notificationChallenge.Id,
            Title: notificationChallenge.Title,
            Description: notificationChallenge.Description,
            UserId: notificationChallenge.UserId,
            RecipientType: notificationChallenge.RecipientType,
            RecipientId: notificationChallenge.RecipientId,
            MetadataJson: notificationChallenge.MetadataJson,
            CreatedAt: notificationChallenge.CreatedAt,
            IsRead: notificationChallenge.Read
        );

        await hubContext.Clients.Group(notificationChallenge.RecipientId.ToString())
            .ReceiveFriendRequestNotification(notificationDto);
    }
}