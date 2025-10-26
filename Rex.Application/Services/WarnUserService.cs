using Microsoft.Extensions.Logging;
using Rex.Application.DTOs.JWT;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Models;

namespace Rex.Application.Services;

public class WarnUserService(
    ILogger<WarnUserService> logger,
    IUserGroupRepository userGroupRepository,
    IInactiveUserNotifier notifier
) : IWarnUserService
{
    public async Task<ResultT<ResponseDto>> ProcessWarning(CancellationToken cancellationToken)
    {
        var users = await userGroupRepository.GetInactiveUserGroupsForWarning(
            InactivityThresholds.WarningDays,
            cancellationToken
        );

        if (users is null)
        {
            logger.LogInformation("No inactive users found for warning.");
            return ResultT<ResponseDto>.Success(new ResponseDto("No inactive users to warn"));
        }

        var processedCount = 0;

        foreach (var user in users)
        {
            var notification = new Notification
            {
                Title = "We Miss You in the Group!",
                Description =
                    $"Hi {user.User?.FirstName ?? "there"}, it's been over {InactivityThresholds.WarningDays} days since your last post in '{user.Group?.Title ?? "the group"}'. " +
                    $"Please participate soon—if you remain inactive for {InactivityThresholds.RemovalDays} days, you will be automatically removed from the group.",
                UserId = user.UserId,
                RecipientId = user.UserId,
                Read = false,
                CreatedAt = DateTime.UtcNow
            };

            await notifier.SendWarnNotification(notification, cancellationToken);
            await userGroupRepository.MarkAsWarned(user.Id, cancellationToken);

            processedCount++;

            logger.LogInformation("Warning sent to user {UserId} for group {GroupId}",
                user.UserId, user.GroupId);
        }

        logger.LogInformation("Warning process completed for {Count} users", processedCount);

        return ResultT<ResponseDto>.Success(
            new ResponseDto($"Warnings sent to {processedCount} users")
        );
    }
}