using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Challenge;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Challenges.Commands.DeleteChallenge;
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
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class ChallengesController(IMediator mediator, IUserClaimService userClaimService) : ControllerBase
{
    [Authorize(Policy = "StaffOnly")]
    [HttpPost("groups/{groupId}")]
    [SwaggerOperation(
        Summary = "Create a new challenge",
        Description = "Creates a new challenge with the provided data"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> CreateChallengeAsync(
        [FromRoute] Guid groupId,
        [FromForm] CreateChallengeDto createChallenge,
        CancellationToken cancellation)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(
            new CreateChallengeCommand(userId, groupId, createChallenge.Title,
                createChallenge.Description, createChallenge.Duration, createChallenge.CoverPhoto), cancellation);
    }

    [HttpGet("groups/{groupId}/status/{status}")]
    [SwaggerOperation(
        Summary = "Get challenges by status",
        Description = "Returns the challenges of a group filtered by status"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<PagedResult<ChallengeGroupDetailsDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<PagedResult<ChallengeGroupDetailsDto>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<PagedResult<ChallengeGroupDetailsDto>>))]
    public async Task<ResultT<PagedResult<ChallengeGroupDetailsDto>>> GetChallengesByStatusAsync(
        [FromRoute] Guid groupId,
        [FromRoute] ChallengeStatus status,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellation)
    {
        return await mediator.Send(new GetChallengesByStatusQuery(groupId, status, pageNumber, pageSize), cancellation);
    }

    [HttpPost("challenges/{challengeId}/join")]
    [SwaggerOperation(
        Summary = "Join a challenge",
        Description = "Allows the authenticated user to join an existing challenge"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> JoinChallengeAsync(
        [FromRoute] Guid challengeId,
        CancellationToken cancellation)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new JoinChallengeCommand(challengeId, userId), cancellation);
    }

    [Authorize(Policy = "StaffOnly")]
    [HttpPut("groups/{groupId}/challenges/{challengeId}")]
    [SwaggerOperation(
        Summary = "Update a challenge",
        Description = "Updates the information of an existing challenge"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> UpdateChallengeAsync(
        [FromRoute] Guid groupId,
        [FromRoute] Guid challengeId,
        [FromForm] UpdateChallengeDto dto,
        CancellationToken cancellation)
    {
        var command = new UpdateChallengeCommand(groupId, challengeId, dto.Title, dto.Description, dto.Duration,
            dto.Status);
        return await mediator.Send(command, cancellation);
    }

    [HttpGet("user")]
    [SwaggerOperation(
        Summary = "Get challenges of the authenticated user",
        Description = "Returns the challenges associated with the authenticated user filtered by status"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<PagedResult<ChallengeUserDetailsDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<PagedResult<ChallengeUserDetailsDto>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<PagedResult<ChallengeUserDetailsDto>>))]
    public async Task<ResultT<PagedResult<ChallengeUserDetailsDto>>> GetChallengesByUserAsync(
        [FromQuery] UserChallengeStatus status,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new GetChallengesByUserQuery(userId, status, pageNumber, pageSize),
            cancellationToken);
    }

    [Authorize(Policy = "StaffOnly")]
    [HttpDelete("groups/{groupId}/challenges/{challengeId}")]
    [SwaggerOperation(
        Summary = "Delete a challenge",
        Description = "Deletes a challenge if the authenticated user is authorized"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> DeleteChallengeAsync(
        [FromRoute] Guid groupId,
        [FromRoute] Guid challengeId,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new DeleteChallengeCommand(challengeId, groupId, userId), cancellationToken);
    }
}