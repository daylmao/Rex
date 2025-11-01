using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler(
    ILogger<MarkNotificationAsReadCommandHandler> logger,
    INotificationRepository notificationRepository,
    IDistributedCache cache
) : ICommandHandler<MarkNotificationAsReadCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting process to mark notification {NotificationId} as read.",
            request.NotificationId);

        var notification = await notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification is null)
        {
            logger.LogWarning("Notification {NotificationId} not found in the system.", request.NotificationId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404",
                "The notification you are trying to mark as read does not exist."));
        }

        if (notification.Read)
        {
            logger.LogInformation("Notification {NotificationId} is already marked as read.", request.NotificationId);
            return ResultT<ResponseDto>.Success(new ResponseDto("This notification was already marked as read."));
        }

        notification.Read = true;
        notification.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Notification {NotificationId} successfully marked as read.", request.NotificationId);
        await notificationRepository.UpdateAsync(notification, cancellationToken);

        return ResultT<ResponseDto>.Success(new ResponseDto("The notification has been marked as read successfully."));
    }
}