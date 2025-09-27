using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs;
using Rex.Application.Modules.Challenges.Commands.JoinChallenge;
using Rex.Application.Modules.Challenges.Commands.UpdateChallenge;
using Rex.Application.Modules.Challenges.CreateChallenge;
using Rex.Application.Modules.Challenges.Queries.GetChallengesByStatus;
using Rex.Application.Modules.Challenges.Queries.GetChallengesByUser;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ChallengesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new challenge",
        Description = "Creates a new challenge with the provided data"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> CreateChallengeAsync([FromForm] CreateChallengeCommand command,
        CancellationToken cancellation)
    {
        return await mediator.Send(command, cancellation);
    }

    [HttpGet("{groupId}/status/{status}")]
    [SwaggerOperation(
        Summary = "Get challenges by status",
        Description = "Returns the challenges of a group filtered by status"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<ChallengeGroupDetailsDto>>> GetChallengesByStatusAsync(
        [FromRoute] Guid groupId, [FromRoute] ChallengeStatus status,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellation)
    {
        return await mediator.Send(new GetChallengesByStatusQuery(groupId, status, pageNumber, pageSize), cancellation);
    }

    [HttpPost("{challengeId}/join/user/{userId}")]
    [SwaggerOperation(
        Summary = "Join a challenge",
        Description = "Allows a user to join an existing challenge"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> JoinChallengeAsync([FromRoute] Guid challengeId, [FromRoute] Guid userId
        , CancellationToken cancellation)
    {
        return await mediator.Send(new JoinChallengeCommand(challengeId, userId), cancellation);
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "Update a challenge",
        Description = "Updates the information of an existing challenge"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> UpdateChallengeAsync([FromBody] UpdateChallengeCommand command,
        CancellationToken cancellation)
    {
        return await mediator.Send(command, cancellation);
    }

    [HttpGet("user/{userId}")]
    [SwaggerOperation(
        Summary = "Get challenges by user",
        Description = "Returns the challenges associated with a user filtered by status"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<ChallengeUserDetailsDto>>> GetChallengesByUserAsync([FromRoute] Guid userId,
        [FromQuery] UserChallengeStatus status,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetChallengesByUserQuery(userId, status, pageNumber, pageSize),
            cancellationToken);
    }
}