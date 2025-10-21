using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Message;
using Rex.Application.Modules.Messages.Commands.SendFileMessage;
using Rex.Application.Modules.Messages.Queries.GetMessagesByChatId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class MessagesController(IMediator mediator) : ControllerBase
{
    [HttpGet("chat/{chatId}")]
    [SwaggerOperation(
        Summary = "Get chat messages",
        Description = "Retrieves a paginated list of messages from a specific chat."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<MessageDto>>> GetMessageByChatIdAsync(
        [FromRoute] Guid chatId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetMessagesByChatIdQuery(chatId, pageNumber, pageSize), cancellationToken);
    }

    [HttpPost("file")]
    [SwaggerOperation(
        Summary = "Send file message",
        Description = "Sends a message that includes a file attachment within a specific chat."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ResultT<MessageDto>> SendFileMessageAsync(
        [FromForm] SendFileMessageCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }
}