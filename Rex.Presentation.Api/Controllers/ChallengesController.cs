using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.Modules.Challenges.Commands.JoinChallenge;
using Rex.Application.Modules.Challenges.Commands.UpdateChallenge;
using Rex.Application.Modules.Challenges.CreateChallenge;
using Rex.Application.Modules.Challenges.Queries.GetChallengesByStatus;
using Rex.Application.Modules.Challenges.Queries.GetChallengesByUser;
using Rex.Enum;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ChallengesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateChallengeAsync([FromForm] CreateChallengeCommand command,
        CancellationToken cancellation)
    {
        var result = await mediator.Send(command, cancellation);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpGet("{groupId}/status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChallengesByStatusAsync( [FromRoute] Guid groupId, [FromRoute] ChallengeStatus status,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellation)
    {
        var result = await mediator.Send(new GetChallengesByStatusQuery(groupId, status, pageNumber, pageSize), cancellation);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error),
        };
    }
    
    [HttpPost("{challengeId}/join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JoinChallengeAsync([FromRoute] Guid challengeId, [FromRoute] Guid userId 
        ,CancellationToken cancellation)
    {
        var result = await mediator.Send(new JoinChallengeCommand(challengeId, userId), cancellation);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error!.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error),
        };
    }
    
    [HttpPut("{challengeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateChallengeAsync([FromBody] UpdateChallengeCommand command, CancellationToken cancellation)
    {
        var result = await mediator.Send(command, cancellation);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error!.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error),
        };
    }
    
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChallengesByUserAsync([FromRoute] Guid userId, [FromQuery] UserChallengeStatus status,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetChallengesByUserQuery(userId, status, pageNumber, pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error!.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error),
        };
    }

}