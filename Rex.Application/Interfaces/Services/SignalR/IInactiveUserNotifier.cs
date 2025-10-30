using Rex.Models;

namespace Rex.Application.Interfaces.SignalR;

public interface IInactiveUserNotifier
{
    Task SendWarnNotification(Notification notification, CancellationToken cancellationToken);
    Task SendBanNotification(Notification notification, CancellationToken cancellationToken);
}