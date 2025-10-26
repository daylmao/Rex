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

public class RemoveUserService(
    ILogger<RemoveUserService> logger,
    IUserGroupRepository userGroupRepository,
    IInactiveUserNotifier notifier
) : IRemoveUserService
{
    public async Task<ResultT<ResponseDto>> ProcessRemoval(CancellationToken cancellationToken)
    {
        var users = await userGroupRepository.GetInactiveUserGroupsForRemoval(
            InactivityThresholds.RemovalDays,
            cancellationToken
        );

        if (users == null || !users.Any())
        {
            logger.LogInformation("No inactive users found for removal.");
            return ResultT<ResponseDto>.Success(new ResponseDto("No inactive users to remove"));
        }

        var processedCount = 0;

        foreach (var user in users)
        {
            var notification = new Notification
            {
                Title = "Removed from Group Due to Inactivity",
                Description =
                    $"Hi {user.User?.FirstName ?? "there"}, you have been removed from '{user.Group?.Title ?? "the group"}' due to {InactivityThresholds.RemovalDays} days of inactivity. " +
                    $"You're welcome to rejoin if you wish to participate again.",
                UserId = user.UserId,
                RecipientId = user.UserId,
                Read = false,
                CreatedAt = DateTime.UtcNow
            };

            await notifier.SendBanNotification(notification, cancellationToken);

            user.Status = RequestStatus.Removed.ToString();
            await userGroupRepository.UpdateAsync(user, cancellationToken);

            processedCount++;

            logger.LogInformation("User {UserId} removed from group {GroupId} due to inactivity",
                user.UserId, user.GroupId);
        }

        logger.LogInformation("Removal process completed for {Count} users", processedCount);

        return ResultT<ResponseDto>.Success(
            new ResponseDto($"Users removed: {processedCount}")
        );
    }
}