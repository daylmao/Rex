using Microsoft.AspNetCore.Http;
using Rex.Application.DTOs.Message;
using Rex.Application.Utilities;

namespace Rex.Application.Interfaces;

public interface IMessageService
{
    Task<ResultT<MessageDto>> SendMessageAsync(
        Guid chatId,
        Guid userId,
        string messageText,
        IEnumerable<IFormFile>? files = null,
        CancellationToken cancellationToken = default);

}