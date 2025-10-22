using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Friendship;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Friendships.Commands;
using Rex.Application.Modules.Friendships.Commands.DeleteFriendship;
using Rex.Application.Modules.Friendships.Commands.ManageFriendshipRequest;
using Rex.Application.Modules.Friendships.Queries.GetFriendshipsRequest;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class FriendshipsController(IMediator mediator, IUserClaims userClaims) : ControllerBase
{
    [HttpPut("{targetUserId}/status")]
    [SwaggerOperation(
        Summary = "Manage friendship request",
        Description = "Accept or reject a friendship request"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> ManageFriendshipRequest(
        [FromRoute] Guid targetUserId,
        [FromBody] UpdateFriendshipStatusDto dto,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new ManageFriendshipRequestCommand(userId, targetUserId, dto.Status),
            cancellationToken);
    }

    [HttpGet("requests")]
    [SwaggerOperation(
        Summary = "Get friendship requests by user",
        Description = "Retrieves a paginated list of friendship requests received by the specified user."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<FriendshipRequestDto>>> GetFriendshipRequests(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new GetFriendshipsRequestQuery(userId, pageNumber, pageSize), cancellationToken);
    }

    [HttpDelete("{targetUserId}")]
    [SwaggerOperation(
        Summary = "Delete a friendship",
        Description =
            "Deletes a friendship between the authenticated user and the specified target user and deactivates the associated chat"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> DeleteFriendship([FromRoute] Guid targetUserId,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new DeleteFriendshipCommand(userId, targetUserId), cancellationToken);
    }
}