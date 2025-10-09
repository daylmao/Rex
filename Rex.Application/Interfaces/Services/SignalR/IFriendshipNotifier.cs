using Rex.Models;

namespace Rex.Application.Interfaces.SignalR;

public interface IFriendshipNotifier
{
    Task SendFriendRequestNotification(Notification notification, CancellationToken cancellationToken);

}
