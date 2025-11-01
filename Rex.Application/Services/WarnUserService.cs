using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rex.Application.DTOs.JWT;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Enum;
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

        var validUsers = users?.Where(u => u.User != null).ToList();

        if (validUsers == null || !validUsers.Any())
        {
            logger.LogInformation("No inactive users found for warning.");
            return ResultT<ResponseDto>.Success(new ResponseDto("No inactive users to warn"));
        }

        var now = DateTime.UtcNow;
        var userIds = new List<Guid>();

        foreach (var user in validUsers)
        {
            var metadata = new
            {
                GroupId = user.GroupId,
                UserId = user.UserId,
                WarningDays = InactivityThresholds.WarningDays
            };

            var notification = new Notification
            {
                Title = "We Miss You in the Group!",
                Description =
                    $"Hi {user.User.FirstName}, it's been over {InactivityThresholds.WarningDays} days since your last post in '{user.Group?.Title ?? "the group"}'. Please participate soon—if you remain inactive for {InactivityThresholds.RemovalDays} days, you will be automatically removed from the group.",
                UserId = user.UserId,
                RecipientType = TargetType.User.ToString(),
                RecipientId = user.UserId,
                MetadataJson = JsonSerializer.Serialize(metadata),
                Read = false,
                CreatedAt = now
            };

            await notifier.SendWarnNotification(notification, cancellationToken);

            logger.LogInformation("Warning sent to user {UserId} in group {GroupId}", user.UserId, user.GroupId);

            userIds.Add(user.UserId);
        }

        if (userIds.Any())
            await userGroupRepository.MarkMultipleAsWarned(userIds, cancellationToken);

        logger.LogInformation("Warning process completed for {Count} users", userIds.Count);

        return ResultT<ResponseDto>.Success(new ResponseDto($"Warnings sent to {userIds.Count} users"));
    }
}