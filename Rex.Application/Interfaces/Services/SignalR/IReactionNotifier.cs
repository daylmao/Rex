using Rex.Application.DTOs;
using Rex.Application.DTOs.Reaction;
using Rex.Models;

namespace Rex.Application.Interfaces.SignalR;

public interface IReactionNotifier
{
    Task ReactionPostNotificationAsync(Notification notification, CancellationToken cancellationToken);
    Task LikeChangedNotificationAsync(LikeChangedDto dto, CancellationToken cancellationToken);
}