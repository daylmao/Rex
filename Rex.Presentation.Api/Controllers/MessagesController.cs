using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Message;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Messages.Commands.SendFileMessage;
using Rex.Application.Modules.Messages.Queries.GetMessagesByChatId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class MessagesController(IMediator mediator, IUserClaimService userClaimService) : ControllerBase
{
    [HttpGet("chat/{chatId}")]
    [SwaggerOperation(
        Summary = "Get chat messages",
        Description = "Retrieves a paginated list of messages from a specific chat."
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<PagedResult<MessageDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<PagedResult<MessageDto>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<PagedResult<MessageDto>>))]
    public async Task<ResultT<PagedResult<MessageDto>>> GetMessageByChatIdAsync(
        [FromRoute] Guid chatId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetMessagesByChatIdQuery(chatId, pageNumber, pageSize), cancellationToken);
    }

    [Authorize]
    [HttpPost("file")]
    [SwaggerOperation(
        Summary = "Send file message",
        Description = "Sends a message that includes a file attachment within a specific chat."
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<MessageDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<MessageDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<MessageDto>))]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType, Type = typeof(ResultT<MessageDto>))]
    public async Task<ResultT<MessageDto>> SendFileMessageAsync(
        [FromForm] SendFileMessageDto sendFileMessage,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(
            new SendFileMessageCommand(sendFileMessage.ChatId, userId, sendFileMessage.Message, sendFileMessage.Files),
            cancellationToken);
    }
}