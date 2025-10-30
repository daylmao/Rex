using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Challenge;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Chats.Queries.GetChatsByUserId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ChatsController(IMediator mediator, IUserClaimService userClaimService) : ControllerBase
{
    
    [Authorize]
    [HttpGet] 
    [SwaggerOperation(
        Summary = "Get user chats",
        Description = "Retrieves a paginated list of chats for the authenticated user, optionally filtered by a search term."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<ChatLastMessageDto>>> GetChats(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new GetChatsByUserIdQuery(userId, page, pageSize, searchTerm), cancellationToken);
    }
}
