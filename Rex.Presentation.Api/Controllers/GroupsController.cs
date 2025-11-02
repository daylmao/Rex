using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Group;
using Rex.Application.DTOs.JWT;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Groups.Commands;
using Rex.Application.Modules.Groups.Commands.CreateGroup;
using Rex.Application.Modules.Groups.Commands.DeleteGroup;
using Rex.Application.Modules.Groups.Commands.GroupUserModeration;
using Rex.Application.Modules.Groups.Commands.ManageRequest;
using Rex.Application.Modules.Groups.Commands.RequestToJoinGroup;
using Rex.Application.Modules.Groups.Commands.UpdateGroup;
using Rex.Application.Modules.Groups.Commands.UpdateGroupRoleMember;
using Rex.Application.Modules.Groups.Queries.GetGroupById;
using Rex.Application.Modules.Groups.Queries.GetGroupJoinRequests;
using Rex.Application.Modules.Groups.Queries.GetGroupMembers;
using Rex.Application.Modules.Groups.Queries.GetGroupsByUserId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/groups")]
public class GroupsController(IMediator mediator, IUserClaimService userClaimService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new group",
        Description = "Creates a new group with the provided data"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDto))]
    public async Task<ResultT<ResponseDto>> CreateGroupsAsync([FromForm] CreateGroupDto createGroup,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(
            new CreateGroupCommand(userId, createGroup.ProfilePhoto, createGroup.CoverPhoto, createGroup.Title,
                createGroup.Description, createGroup.Visibility), cancellationToken);
    }

    [HttpGet("{groupId}/membership")]
    [SwaggerOperation(
        Summary = "Get group by ID",
        Description = "Returns detailed information of a group for a specific user"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GroupDetailsDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDto))]
    public async Task<ResultT<GroupDetailsDto>> GetGroupById([FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new GetGroupByGroupIdQuery(groupId, userId), cancellationToken);
    }

    [HttpGet("my-groups")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<GroupDetailsDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GroupDetailsDto))]
    public async Task<ResultT<PagedResult<GroupDetailsDto>>> GetGroupsByUserId(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new GetGroupsByUserIdQuery(userId, pageNumber, pageSize), cancellationToken);
    }

    [Authorize("LeaderOrModerator")]
    [HttpPut("{groupId}")]
    [SwaggerOperation(
        Summary = "Update group information",
        Description = "Updates information of an existing group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDto))]
    public async Task<ResultT<ResponseDto>> UpdateGroupInformation([FromRoute] Guid groupId,
        [FromForm] UpdateGroupDto updateGroupDto,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(
            new UpdateGroupCommand(groupId, updateGroupDto.ProfilePhoto, updateGroupDto.CoverPhoto,
                updateGroupDto.Title, updateGroupDto.Description, updateGroupDto.Visibility), cancellationToken);
    }

    [HttpGet("{groupId}/members")]
    [SwaggerOperation(
        Summary = "Get group members",
        Description = "Returns paginated members of a group with optional search and role filtering"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<UserGroupDetailsDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(UserGroupDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UserGroupDetailsDto))]
    public async Task<ResultT<PagedResult<UserGroupDetailsDto>>> GetGroupMembers([FromRoute] Guid groupId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? searchTerm = null,
        [FromQuery] GroupRole? roleFilter = null)
    {
        return await mediator.Send(new GetGroupMembersQuery(groupId, pageNumber, pageSize,
            searchTerm, roleFilter));
    }

    [Authorize("StaffOnly")]
    [HttpGet("{groupId}/join-requests")]
    [SwaggerOperation(
        Summary = "Get join requests",
        Description = "Returns paginated join requests for a group with optional search term"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<UserGroupRequestDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(UserGroupRequestDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UserGroupRequestDto))]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ResponseDto))]
    public async Task<ResultT<ResponseDto>> RequestToJoinGroup([FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new RequestToJoinGroupCommand(userId, groupId), cancellationToken);
    }

    [Authorize("StaffOnly")]
    [HttpPatch("{groupId}/requests/{targetUserId}")]
    [SwaggerOperation(
        Summary = "Update join request status",
        Description = "Approves or rejects a user's request to join a specific group. " +
                      "The new status must be provided in the request body."
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ResponseDto))]
    public async Task<ResultT<ResponseDto>> ManageRequest(
        Guid groupId,
        Guid targetUserId,
        [FromBody] ManageRequestStatusDto statusDto,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(
            new ManageRequestCommand(targetUserId, groupId, statusDto.Status),
            cancellationToken
        );
    }

    [Authorize("LeaderOrModerator")]
    [HttpPatch("{groupId}/members/{memberId}/role")]
    [SwaggerOperation(
        Summary = "Update group member role",
        Description = "Updates the role of a specific user in the selected group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDto))]
    public async Task<ResultT<ResponseDto>> UpdateMemberRole([FromRoute] Guid groupId, [FromRoute] Guid memberId,
        [FromBody] UpdateRoleMemberdto dto,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdateGroupRoleMemberCommand(memberId, groupId, dto.Role), cancellationToken);
    }

    [Authorize("LeaderOnly")]
    [HttpDelete("{groupId}")]
    [SwaggerOperation(
        Summary = "Delete a group",
        Description = "Deletes a group if the request comes from the group leader"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDto))]
    public async Task<ResultT<ResponseDto>> DeleteGroup([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new DeleteGroupCommand(groupId, userId), cancellationToken);
    }

    [Authorize("LeaderOrModerator")]
    [HttpPatch("{groupId}/members/{memberId}/moderate")]
    [SwaggerOperation(
        Summary = "Ban or remove a group member",
        Description = "Ban a user or remove them from the group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDto))]
    public async Task<ResultT<ResponseDto>> ModerateMember([FromRoute] Guid groupId, [FromRoute] Guid memberId,
        [FromBody] GroupUserModerationDto moderation, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GroupUserModerationCommand(memberId, groupId, moderation.Status),
            cancellationToken);
    }
}