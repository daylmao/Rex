using Microsoft.AspNetCore.SignalR;

namespace Rex.Application.Interfaces.SignalR;

public interface IChatConnectionService
{
    Task HandleConnectedAsync(HubCallerContext context, IGroupManager groupManager);
    Task HandleDisconnectedAsync(HubCallerContext context);
}