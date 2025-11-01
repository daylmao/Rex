using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Notification;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Notifications.Queries.GetNotificationsByUserId;

public class GetNotificationsByUserIdCommandHandler(
    ILogger<GetNotificationsByUserIdCommandHandler> logger,
    INotificationRepository notificationRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : IQueryHandler<GetNotificationsByUserIdCommand, PagedResult<NotificationDto>>
{
    public async Task<ResultT<PagedResult<NotificationDto>>> Handle(GetNotificationsByUserIdCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {UserId} not found in the system.", request.UserId);
            return ResultT<PagedResult<NotificationDto>>.Failure(Error.NotFound("404",
                "The user for whom notifications are being requested does not exist."));
        }

        logger.LogInformation("Fetching notifications for user {UserId}.", request.UserId);
        
        var notifications = await cache.GetOrCreateAsync(
            $"notifications:user:{request.UserId}:page:{request.PageNumber}:size:{request.PageSize}",
            async () =>
                await notificationRepository.GetNotificationsByUserIdAsync(
                    request.UserId,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );

        if (notifications is null)
        {
            logger.LogWarning("No notifications found for user {UserId}.", request.UserId);
            return ResultT<PagedResult<NotificationDto>>.Success(new PagedResult<NotificationDto>([],
                notifications.TotalItems, notifications.ActualPage, notifications.TotalPages));
        }

        var notificationDtos = notifications.Items.Select(n => new NotificationDto(
            n.Id,
            n.Title,
            n.Description,
            n.UserId,
            n.RecipientType,
            n.RecipientId,
            n.MetadataJson,
            n.CreatedAt,
            n.Read
        )).ToList();
        
        logger.LogInformation("Fetched {Count} notifications for user {UserId}.", notificationDtos.Count,
            request.UserId);
        
        return ResultT<PagedResult<NotificationDto>>.Success(new PagedResult<NotificationDto>(
            notificationDtos,
            notifications.TotalItems,
            notifications.ActualPage,
            notifications.TotalPages));
    }
}