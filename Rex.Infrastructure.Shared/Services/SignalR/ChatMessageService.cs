using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Models;

namespace Rex.Infrastructure.Shared.Services.SignalR;

public class ChatMessageService(
    ILogger<ChatMessageService> logger,
    IUserChatRepository userChatRepository,
    IMessageRepository messageRepository,
    IChatRepository chatRepository,
    IUserRepository userRepository
) : IChatMessageService
{
    public async Task SendMessageAsync(Guid chatId, string message, ClaimsPrincipal? user, IChatHub group,
        IChatHub caller)
    {
        var userId = GetUserIdFromClaims(user);

        if (userId is null)
        {
            await caller.SendMessageResult(new (false, "You need to be logged in to send messages."));
            return;
        }

        var belongs = await userChatRepository.IsUserInChatAsync(userId.Value, chatId, CancellationToken.None);
        if (!belongs)
        {
            await caller.SendMessageResult(new (false, "You don't have access to this chat."));
            return;
        }
        
        var sender = await userRepository.GetByIdAsync(userId.Value, CancellationToken.None);
        
        var msg = new Message
        {
            ChatId = chatId,
            SenderId = userId.Value,
            Description = message,
        };

        await messageRepository.CreateAsync(msg, CancellationToken.None);
        
        var dto = new MessageDto(
            msg.ChatId,
            msg.SenderId,
            sender.FirstName + " " + sender.LastName,
            sender.ProfilePhoto,
            msg.Description,
            msg.CreatedAt
        );

        await group.ReceiveMessage(dto);
        logger.LogInformation("User {UserId} sent message to chat {ChatId}", userId, chatId);
    }

    private Guid? GetUserIdFromClaims(ClaimsPrincipal? user)
    {
        var claim = user?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is null) return null;
        return Guid.TryParse(claim.Value, out var id) ? id : null;
    }
}