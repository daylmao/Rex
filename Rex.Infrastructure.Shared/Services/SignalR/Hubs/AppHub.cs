using MediatR;
using Microsoft.AspNetCore.SignalR;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Modules.Chats.Commands.CreatePrivateChat;
using Rex.Application.Modules.Friendships.Commands;
using Rex.Application.Modules.Messages.Commands.SendMessage;
using Rex.Application.Modules.Reactions.Commands.AddLike;
using Rex.Application.Modules.Reactions.Commands.RemoveLike;
using Rex.Enum;

namespace Rex.Infrastructure.Shared.Services.SignalR.Hubs;

public class AppHub(
    IAppConnectionService connectionService,
    IUserClaims userClaims,
    IMediator mediator) : Hub<IAppHub>
{
    public override async Task OnConnectedAsync()
        => await connectionService.HandleConnectedAsync(Context, Groups);

    public override async Task OnDisconnectedAsync(Exception? exception)
        => await connectionService.HandleDisconnectedAsync(Context);

    public async Task JoinPostGroup(Guid postId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, postId.ToString());
    }

    public async Task LeavePostGroup(Guid postId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId.ToString());
    }

    public async Task SendMessage(Guid chatId, string message)
    {
        var userId = userClaims.GetUserId(Context.User);
        var result = await mediator.Send(new SendMessageCommand(chatId, message, userId));

        if (!result.IsSuccess)
        {
            await Clients.Caller.ReceiveError(result.Error!.Description);
        }
    }

    public async Task CreatePrivateChat(Guid otherUserId)
    {
        var userId = userClaims.GetUserId(Context.User);
        var result = await mediator.Send(new CreatePrivateChatCommand(userId, otherUserId));

        if (!result.IsSuccess)
        {
            await Clients.Caller.ReceiveError(result.Error!.Description);
        }
    }

    public async Task CreateFriendshipRequest(Guid otherUserId)
    {
        var userId = userClaims.GetUserId(Context.User);
        
        var result = await mediator.Send(new CreateFriendshipRequestCommand(userId, otherUserId));
        if (!result.IsSuccess)
        {
            await Clients.Caller.ReceiveError(result.Error!.Description);
        }
    }

    public async Task AddLike(Guid postId)
    {
        var userId = userClaims.GetUserId(Context.User);

        var result = await mediator.Send(new AddLikeCommand(userId, postId, ReactionTargetType.Post));

        if (!result.IsSuccess)
        {
            await Clients.Caller.ReceiveError(result.Error!.Description);
        }
    }

    public async Task RemoveLike(Guid postId)
    {
        var userId = userClaims.GetUserId(Context.User);

        var result = await mediator.Send(new RemoveLikeCommand(userId, postId, ReactionTargetType.Post));

        if (!result.IsSuccess)
        {
            await Clients.Caller.ReceiveError(result.Error!.Description);
        }
    }
}