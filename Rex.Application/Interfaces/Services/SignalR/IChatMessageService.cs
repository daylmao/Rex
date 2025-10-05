using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Rex.Application.Interfaces.SignalR;

public interface IChatMessageService
{
    Task SendMessageAsync(Guid chatId, string message, ClaimsPrincipal? user, IChatHub group, IChatHub caller);
}