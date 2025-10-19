using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Friendship;
using Rex.Application.DTOs.JWT;
using Rex.Application.Modules.Friendships.Commands;
using Rex.Application.Modules.Friendships.Commands.DeleteFriendship;
using Rex.Application.Modules.Friendships.Commands.ManageFriendshipRequest;
using Rex.Application.Modules.Friendships.Queries.GetFriendshipsRequest;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class FriendshipsController(IMediator mediator) : ControllerBase
{
    [HttpPut("status")]
    [SwaggerOperation(
        Summary = "Manage friendship request",
        Description = "Accept or reject a friendship request"
        )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> ManageFriendshipRequest([FromBody] ManageFriendshipRequestCommand command , CancellationToken cancellationToken)
    {
        return await mediator.Send(command , cancellationToken);
    }

    [HttpGet("user/{userId}")]
    public async Task<ResultT<PagedResult<FriendshipRequestDto>>> GetFriendshipRequests([FromRoute] Guid userId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetFriendshipsRequestQuery(userId, pageNumber, pageSize), cancellationToken);
    }
    
    [HttpDelete]
    [SwaggerOperation(
        Summary = "Delete a friendship",
        Description = "Deletes a friendship between two users and deactivates the associated chat"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> DeleteFriendship([FromBody] DeleteFriendshipCommand command, CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

}