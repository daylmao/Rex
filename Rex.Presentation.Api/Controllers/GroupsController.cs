using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.Modules.Groups.Commands;
using Rex.Application.Modules.Groups.Queries.GetGroupById;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/groups")]
public class GroupsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGroupsAsync([FromForm] CreateGroupCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Error);
    }
    
    [HttpGet("{groupId}/users/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupById([FromRoute] Guid userId, [FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetGroupByIdQuery(groupId, userId), cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
}