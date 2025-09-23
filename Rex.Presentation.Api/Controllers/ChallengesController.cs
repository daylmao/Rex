using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.Modules.Challenges.CreateChallenge;
using Rex.Application.Modules.Challenges.Queries.GetChallengesByStatus;
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

}