using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs;
using Rex.Application.Modules.Messages.Queries.GetMessagesByUserId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class MessagesController(IMediator mediator) : ControllerBase
{
    [HttpGet("chats/{chatId}")]
    public async Task<ResultT<PagedResult<MessageDto>>> GetMessageByChatIdAsync([FromRoute] Guid chatId, [FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetMessagesByUserIdQuery(chatId, pageNumber, pageSize), cancellationToken);
    }
}