using Microsoft.AspNetCore.SignalR;
using Rex.Application.DTOs.Notification;
using Rex.Application.DTOs.Reaction;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Infrastructure.Shared.Services.SignalR.Hubs;
using Rex.Models;

namespace Rex.Infrastructure.Shared.Services.SignalR;

public class ReactionNotifier(
    IHubContext<AppHub, IAppHub> hubContext,
    INotificationRepository notificationRepository
) : IReactionNotifier
{
    public async Task ReactionPostNotificationAsync(Notification notification, CancellationToken cancellationToken)
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
        
        await hubContext.Clients.User(notificationDto.RecipientId.ToString())
            .ReceiveReactionNotification(notificationDto);
    }
    
    public async Task LikeChangedNotificationAsync(LikeChangedDto dto, CancellationToken cancellationToken)
    {
        await hubContext.Clients.Group(dto.PostId.ToString()).ReceiveLikeUpdate(dto);
        
    }
}