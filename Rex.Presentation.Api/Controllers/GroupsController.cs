using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Group;
using Rex.Application.DTOs.JWT;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Groups.Commands;
using Rex.Application.Modules.Groups.Commands.AproveRequest;
using Rex.Application.Modules.Groups.Commands.DeleteGroup;
using Rex.Application.Modules.Groups.Commands.RejectRequest;
using Rex.Application.Modules.Groups.Commands.RequestToJoinGroupCommand;
using Rex.Application.Modules.Groups.Commands.UpdateGroup;
using Rex.Application.Modules.Groups.Commands.UpdateGroupRoleMember;
using Rex.Application.Modules.Groups.Queries.GetGroupById;
using Rex.Application.Modules.Groups.Queries.GetGroupJoinRequests;
using Rex.Application.Modules.Groups.Queries.GetGroupMembers;
using Rex.Application.Modules.Groups.Queries.GetGroupsByUserId;
using Rex.Application.Modules.Posts.Commands;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/groups")]
public class GroupsController(IMediator mediator, IUserClaims userClaims) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new group",
        Description = "Creates a new group with the provided data"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ResultT<ResponseDto>> CreateGroupsAsync([FromForm] CreateGroupDto createGroup,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(
            new CreatePostCommand(createGroup.GroupId, userId, createGroup.ChallengeId, createGroup.Title,
                createGroup.Description, createGroup.Files), cancellationToken);
    }

    [HttpGet("{groupId}/membership")]
    [SwaggerOperation(
        Summary = "Get group by ID",
        Description = "Returns detailed information of a group for a specific user"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<GroupDetailsDto>> GetGroupById([FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new GetGroupByGroupIdQuery(groupId, userId), cancellationToken);
    }

    [HttpGet("my-groups")]
    public async Task<ResultT<PagedResult<GroupDetailsDto>>> GetGroupsByUserId(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new GetGroupsByUserIdQuery(userId, pageNumber, pageSize), cancellationToken);
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "Update group information",
        Description = "Updates information of an existing group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> UpdateGroupInformation([FromForm] UpdateGroupCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("{groupId}/members")]
    [SwaggerOperation(
        Summary = "Get group members",
        Description = "Returns paginated members of a group with optional search and role filtering"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<UserGroupDetailsDto>>> GetGroupMembers([FromRoute] Guid groupId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? searchTerm = null,
        [FromQuery] GroupRole? roleFilter = null)
    {
        return await mediator.Send(new GetGroupMembersQuery(groupId, pageNumber, pageSize,
            searchTerm, roleFilter));
    }

    [HttpGet("{groupId}/join-requests")]
    [SwaggerOperation(
        Summary = "Get join requests",
        Description = "Returns paginated join requests for a group with optional search term"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<UserGroupRequestDto>>> GetGroupJoinRequests([FromRoute] Guid groupId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetGroupJoinRequestsCommand(
            groupId, pageNumber, pageSize, searchTerm), cancellationToken);
    }

    [HttpPost("{groupId}/join-requests")]
    [SwaggerOperation(
        Summary = "Request to join a group",
        Description = "Allows a user to send a request to join a specific group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> RequestToJoinGroup([FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new RequestToJoinGroupCommand(userId, groupId), cancellationToken);
    }

    [HttpPost("{groupId}/join-requests/approve")]
    [SwaggerOperation(
        Summary = "Approve join request",
        Description = "Approves a user's request to join a specific group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> ApproveRequest([FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new ApproveRequestCommand(userId, groupId), cancellationToken);
    }
    
    [HttpPost("{groupId}/join-requests/reject")]
    [SwaggerOperation(
        Summary = "Reject join request",
        Description = "Rejects a user's request to join a specific group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> RejectRequest([FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new RejectRequestCommand(userId, groupId), cancellationToken);
    }


    [HttpPut("role")]
    [SwaggerOperation(
        Summary = "Update group member role",
        Description = "Updates the role of a specific user in the selected group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> UpdateGroupMemberRole(
        [FromBody] UpdateGroupRoleMemberDto updateGroupRoleMember,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(
            new UpdateGroupRoleMemberCommand(userId, updateGroupRoleMember.GroupId, updateGroupRoleMember.Role),
            cancellationToken);
    }

    [HttpDelete]
    [SwaggerOperation(
        Summary = "Delete a group",
        Description = "Deletes a group if the request comes from the group leader"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ResultT<ResponseDto>> DeleteGroupAsync([FromBody] DeleteGroupDto deleteGroup,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new DeleteGroupCommand(deleteGroup.GroupId, userId), cancellationToken);
    }
}