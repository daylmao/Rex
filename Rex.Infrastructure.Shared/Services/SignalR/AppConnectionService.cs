using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;

namespace Rex.Infrastructure.Shared.Services.SignalR;

public class ChatConnectionService(
    ILogger<ChatConnectionService> logger,
    IUserChatRepository userChatRepository,
    IUserRepository userRepository
) : IChatConnectionService
{
    public async Task HandleConnectedAsync(HubCallerContext context, IGroupManager groups)
    {
        var userId = GetUserIdFromClaims(context);
        if (userId is null)
        {
            logger.LogWarning("User attempted to connect but no valid user ID was found.");
            context.Abort();
            return;
        }

        logger.LogInformation("User {UserId} connected with ConnectionId {ConnectionId}", userId, context.ConnectionId);

        var chats = await userChatRepository.GetUserChatsAsync(userId.Value, CancellationToken.None);
        logger.LogInformation("User {UserId} has access to {ChatCount} chats", userId.Value, chats.Count);

        var user = await userRepository.GetByIdAsync(userId.Value, CancellationToken.None);
        if (user is not null)
        {
            user.IsActive = true;
            await userRepository.UpdateAsync(user, CancellationToken.None);
            logger.LogInformation("User {UserId} marked as active", user.Id);
        }

        foreach (var chatId in chats)
        {
            await groups.AddToGroupAsync(context.ConnectionId, chatId.ToString());
            logger.LogDebug("User {UserId} added to chat group {ChatId}", userId.Value, chatId);
        }

        logger.LogInformation("User {UserId} fully connected", userId.Value);
    }

    public async Task HandleDisconnectedAsync(HubCallerContext context)
    {
        var userId = GetUserIdFromClaims(context);
        if (userId is null) return;

        var user = await userRepository.GetByIdAsync(userId.Value, CancellationToken.None);
        if (user is not null)
        {
            user.IsActive = false;
            user.LastConnection = DateTime.UtcNow;
            await userRepository.UpdateAsync(user, CancellationToken.None);
            logger.LogInformation("User {UserId} disconnected", user.Id);
        }
    }

    private Guid? GetUserIdFromClaims(HubCallerContext context)
    {
        var claim = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim is null) return null;
        return Guid.TryParse(claim.Value, out var id) ? id : null;
    }
}
