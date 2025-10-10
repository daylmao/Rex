using Rex.Application.DTOs.Message;
using Rex.Models;

namespace Rex.Application.Interfaces.SignalR;

public interface IChatNotifier
{
    Task NotifyChatCreatedAsync(IEnumerable<Guid> userIds, Chat chat, CancellationToken cancellationToken);
    Task NotifyMessageAsync(Guid chatId, MessageDto message);
}