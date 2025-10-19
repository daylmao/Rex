using Rex.Models;

namespace Rex.Application.Interfaces.SignalR;

public interface IChallengeNotifier
{
    Task SendChallengeNotification(Notification notificationChallenge, CancellationToken cancellation);
}