using Rex.Application.DTOs;

namespace Rex.Application.Interfaces.SignalR;

public interface IAppHub
{
    Task ReceiveMessage(MessageDto message);
    Task ReceiveFriendRequestNotification(NotificationDto notification);
    Task ReceiveReactionNotification(NotificationDto notification);
    Task ReceiveChatCreated(ChatDto chat);
}