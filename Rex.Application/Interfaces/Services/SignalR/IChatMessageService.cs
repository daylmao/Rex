using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Rex.Application.Interfaces.SignalR;

public interface IChatMessageService
{
    Task SendMessageAsync(Guid chatId, string message, ClaimsPrincipal? user, IChatHub group, IChatHub caller, CancellationToken cancellationToken, List<IFormFile>? imageUrl = null);
}