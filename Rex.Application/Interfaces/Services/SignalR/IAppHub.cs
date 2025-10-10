using Rex.Application.DTOs.Challenge;
using Rex.Application.DTOs.Message;
using Rex.Application.DTOs.Notification;

namespace Rex.Application.Interfaces.SignalR;

public interface IAppHub
{
    Task ReceiveMessage(MessageDto message);
    Task ReceiveFriendRequestNotification(NotificationDto notification);
    Task ReceiveReactionNotification(NotificationDto notification);
    Task ReceiveChatCreated(ChatDto chat);
}