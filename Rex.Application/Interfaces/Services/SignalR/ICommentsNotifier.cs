using Rex.Models;

namespace Rex.Application.Interfaces.SignalR;

public interface ICommentsNotifier
{
    Task SendCommentNotification(Notification notificationComment, CancellationToken cancellationToken);
    Task SendReplyNotification(Notification notificationReply, CancellationToken cancellationToken);
}