using Microsoft.AspNetCore.SignalR;
using Rex.Application.Interfaces.SignalR;

namespace Rex.Infrastructure.Shared.Services.SignalR.Hubs;

public class ChatHub(
    IChatConnectionService connectionService,
    IChatMessageService messageService
) : Hub<IChatHub>
{
    public override async Task OnConnectedAsync()
        => await connectionService.HandleConnectedAsync(Context, Groups);

    public override async Task OnDisconnectedAsync(Exception? exception)
        => await connectionService.HandleDisconnectedAsync(Context);

    public async Task SendMessage(Guid chatId, string message)
        => await messageService.SendMessageAsync(chatId, message, Context.User, Clients.Group(chatId.ToString()), Clients.Caller);
}