using Rex.Application.DTOs.Challenge;
using Rex.Application.DTOs.Message;
using Rex.Application.DTOs.Notification;
using Rex.Application.DTOs.Reaction;

namespace Rex.Application.Interfaces.SignalR;

public interface IAppHub
{
    Task ReceiveMessage(MessageDto message);
    Task ReceiveFriendRequestNotification(NotificationDto notification);
    Task ReceiveReactionNotification(NotificationDto notification);
    Task ReceiveCommentNotification(NotificationDto notification);
    Task ReceiveReplyNotification(NotificationDto notification);
    
    Task ReceiveChatCreated(ChatDto chat);
    Task ReceiveError(string error);

    Task CreateFriendshipRequest(Guid otherUserId);
    
    Task ReceiveLikeUpdate(LikeChangedDto update);
}