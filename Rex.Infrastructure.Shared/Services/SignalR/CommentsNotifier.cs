using Microsoft.AspNetCore.SignalR;
using Rex.Application.DTOs.Notification;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Infrastructure.Shared.Services.SignalR.Hubs;
using Rex.Models;

namespace Rex.Infrastructure.Shared.Services.SignalR;

public class CommentsNotifier(
    IHubContext<AppHub, IAppHub> hubContext,
    INotificationRepository notificationRepository
) : ICommentsNotifier
{
    public async Task SendCommentNotification(Notification notificationComment, CancellationToken cancellationToken)
    {
        await notificationRepository.CreateAsync(notificationComment, cancellationToken);

        var notificationDto = new NotificationDto(
            Id: notificationComment.Id,
            Title: notificationComment.Title,
            Description: notificationComment.Description,
            UserId: notificationComment.UserId,
            RecipientType: notificationComment.RecipientType,
            RecipientId: notificationComment.RecipientId,
            MetadataJson: notificationComment.MetadataJson,
            CreatedAt: notificationComment.CreatedAt,
            IsRead: notificationComment.Read
        );

        await hubContext.Clients.User(notificationComment.RecipientId.ToString())
            .ReceiveCommentNotification(notificationDto);
    }

    public async Task SendReplyNotification(Notification notificationReply, CancellationToken cancellationToken)
    {
        await notificationRepository.CreateAsync(notificationReply, cancellationToken);
    
        var notificationDto = new NotificationDto(
            Id: notificationReply.Id,
            Title: notificationReply.Title,
            Description: notificationReply.Description,
            UserId: notificationReply.UserId,
            RecipientType: notificationReply.RecipientType,
            RecipientId: notificationReply.RecipientId, 
            MetadataJson: notificationReply.MetadataJson,
            CreatedAt: notificationReply.CreatedAt,
            IsRead: notificationReply.Read
        );

        await hubContext.Clients.User(notificationDto.RecipientId.ToString())
            .ReceiveReplyNotification(notificationDto);
    }
}