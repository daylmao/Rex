using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs;
using Rex.Application.Modules.Groups.Commands;
using Rex.Application.Modules.Groups.Commands.AproveRequest;
using Rex.Application.Modules.Groups.Commands.RejectRequest;
using Rex.Application.Modules.Groups.Commands.RequestToJoinGroupCommand;
using Rex.Application.Modules.Groups.Commands.UpdateGroup;
using Rex.Application.Modules.Groups.Queries.GetGroupById;
using Rex.Application.Modules.Groups.Queries.GetGroupJoinRequests;
using Rex.Application.Modules.Groups.Queries.GetGroupMembers;
using Rex.Application.Modules.Groups.Queries.GetGroupsByUserId;
using Rex.Application.Modules.Groups.Queries.GetGroupsPaginated;
using Rex.Enum;

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
    public async Task<IActionResult> UpdateGroupInformation([FromForm] UpdateGroupCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpGet("{groupId}/members")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupMembers([FromRoute] Guid groupId, [FromQuery] int pageNumber,
        [FromQuery] int pageSize, [FromQuery] string? searchTerm = null, [FromQuery] GroupRole? roleFilter = null)
    {
        var result = await mediator.Send(new GetGroupMembersQuery(groupId, pageNumber, pageSize,
            searchTerm, roleFilter));

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpGet("{groupId}/join-requests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupJoinRequests([FromRoute] Guid groupId, [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetGroupJoinRequestsCommand(
            groupId, pageNumber, pageSize, searchTerm), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPost("{groupId}/join-requests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RequestToJoinGroup([FromRoute] Guid groupId, [FromBody] RequestToJoinGroupDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RequestToJoinGroupCommand(request.UserId, groupId), cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            "409" => Conflict(result.Error),
            _ => BadRequest(result.Error)
        };
    }
    
    [HttpPost("{groupId}/join-requests/{userId}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveRequest([FromRoute] Guid groupId, [FromRoute] Guid userId, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ApproveRequestCommand(userId, groupId), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            "409" => Conflict(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPost("{groupId}/join-requests/{userId}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectRequest([FromRoute] Guid groupId, [FromRoute] Guid userId, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RejectRequestCommand(userId, groupId), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            "409" => Conflict(result.Error),
            _ => BadRequest(result.Error)
        };
    }
    
}