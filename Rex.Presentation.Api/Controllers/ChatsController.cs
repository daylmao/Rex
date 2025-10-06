using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs;
using Rex.Application.Modules.Chats.Commands.CreatePrivateChat;
using Rex.Application.Modules.Chats.Queries.GetChatsByUserId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ChatsController(IMediator mediator) : ControllerBase
{

    [HttpGet("user/{userId}")]
    public async Task<ResultT<PagedResult<ChatLastMessageDto>>> GetChatsByUserId(
        [FromRoute] Guid userId,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetChatsByUserIdQuery(userId, page, pageSize);
        return await mediator.Send(query, cancellationToken);
    }
}