using Microsoft.AspNetCore.SignalR;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Models;

namespace Rex.Infrastructure.Shared.Services.SignalR;

public class ChatNotifier(
    IHubContext<ChatHub, IChatHub> hubContext,
    IUserChatRepository userChatRepository
) : IChatNotifier
{
    public async Task NotifyChatCreatedAsync(IEnumerable<Guid> userIds, Chat chat, CancellationToken cancellationToken)
    {
        foreach (var userId in userIds)
        {
            var otherUser = await userChatRepository.GetOtherUserInChatAsync(chat.Id, userId, cancellationToken);
            var chatDto = new ChatDto(chat.Id, otherUser.FirstName + " " + otherUser.LastName, chat.Type,
                otherUser.ProfilePhoto);
            await hubContext.Clients.User(userId.ToString()).ReceiveChatCreated(chatDto);
        }
    }

    public async Task NotifyMessageAsync(Guid chatId, MessageDto message)
    {
        await hubContext.Clients.Group(chatId.ToString())
            .ReceiveMessage(message);
    }
}