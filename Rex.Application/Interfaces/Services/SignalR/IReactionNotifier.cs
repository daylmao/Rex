using Rex.Application.DTOs;
using Rex.Models;

namespace Rex.Application.Interfaces.SignalR;

public interface IReactionNotifier
{
    Task ReactionPostNotifier(Notification notification, CancellationToken cancellationToken);
}