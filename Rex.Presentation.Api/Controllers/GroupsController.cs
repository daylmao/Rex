using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.Modules.Groups.Commands;
using Rex.Application.Modules.Groups.Commands.UpdateGroup;
using Rex.Application.Modules.Groups.Queries.GetGroupById;
using Rex.Application.Modules.Groups.Queries.GetGroupsByUserId;
using Rex.Application.Modules.Groups.Queries.GetGroupsPaginated;

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
    
    [HttpGet("{groupId}/membership/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId, [FromRoute] Guid userId, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetGroupByGroupIdQuery(groupId, userId), cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
    
    [HttpGet("by-user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupsByUserId([FromRoute] Guid userId, [FromQuery] int pageNumber,
        int pageSize, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetGroupsByUserIdQuery(userId, pageNumber, pageSize), cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
    
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGroupInformation([FromForm] UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command,cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
}