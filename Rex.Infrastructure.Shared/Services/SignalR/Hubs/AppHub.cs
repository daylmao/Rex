using MediatR;
using Microsoft.AspNetCore.SignalR;
using Rex.Application.Helpers;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Modules.Chats.Commands.CreatePrivateChat;
using Rex.Application.Modules.Messages.Commands.SendMessage;

namespace Rex.Infrastructure.Shared.Services.SignalR.Hubs;

public class AppHub(
    IAppConnectionService connectionService,
    IMediator mediator) : Hub<IAppHub>
{
    public override async Task OnConnectedAsync()
        => await connectionService.HandleConnectedAsync(Context, Groups);

    public override async Task OnDisconnectedAsync(Exception? exception)
        => await connectionService.HandleDisconnectedAsync(Context);
    
    public async Task SendMessage(Guid chatId, string message)
    {
        var userId = UserClaims.GetUserId(Context.User);

        await mediator.Send(new SendMessageCommand(chatId, message, userId.Value));
    }
    
    public async Task CreatePrivateChat(Guid otherUserId)
    {
        var userId = UserClaims.GetUserId(Context.User);
        
        await mediator.Send(new CreatePrivateChatCommand(userId.Value, otherUserId));
    }

}