using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;
using GroupRole = Rex.Enum.GroupRole;

namespace Rex.Application.Modules.Groups.Commands.RequestToJoinGroupCommand;

public class RequestToJoinGroupCommandHandler(
    ILogger<RequestToJoinGroupCommandHandler> logger,
    IUserRepository userRepository,
    IGroupRepository groupRepository,
    IUserGroupRepository userGroupRepository,
    IGroupRoleRepository groupRoleRepository,
    IDistributedCache cache
) : ICommandHandler<RequestToJoinGroupCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(RequestToJoinGroupCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Request to join group command is null");
            return ResultT<ResponseDto>.Failure(
                Error.Failure("400", "Oops! No request data was provided."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found", request.UserId);
            return ResultT<ResponseDto>.Failure(
                Error.NotFound("404", "We couldn’t find your account."));
        }

        var group = await groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group with ID {GroupId} not found", request.GroupId);
            return ResultT<ResponseDto>.Failure(
                Error.NotFound("404", "The group you are trying to join does not exist."));
        }

        var isUserInGroup = await userGroupRepository.IsUserInGroupAsync(
            request.UserId, request.GroupId, RequestStatus.Accepted, cancellationToken);
        if (isUserInGroup)
        {
            logger.LogWarning("User {UserId} is already a member of group {GroupId}", request.UserId, request.GroupId);
            return ResultT<ResponseDto>.Failure(
                Error.Conflict("409", "You are already a member of this group."));
        }

        var isUserBanned =
            await userGroupRepository.IsUserBannedAsync(request.UserId, request.GroupId, cancellationToken);
        if (isUserBanned)
        {
            logger.LogWarning("User {UserId} is banned from group {GroupId}", request.UserId, request.GroupId);
            return ResultT<ResponseDto>.Failure(
                Error.Conflict("409", "You can’t join this group because you’ve been banned."));
        }

        var pendingRequest =
            await userGroupRepository.RequestExistsAsync(request.UserId, request.GroupId, cancellationToken);
        if (pendingRequest)
        {
            logger.LogWarning("User {UserId} already has a pending request to join group {GroupId}", request.UserId,
                request.GroupId);
            return ResultT<ResponseDto>.Failure(
                Error.Conflict("409", "You already have a pending request for this group. Please wait for approval."));
        }

        var role = await groupRoleRepository.GetRoleByNameAsync(GroupRole.Member, cancellationToken);
        if (role is null)
        {
            logger.LogError("Group role 'Member' not found in the database");
            return ResultT<ResponseDto>.Failure(
                Error.Failure("400", "Something went wrong. Group role 'Member' is missing."));
        }

        var userGroup = new UserGroup
        {
            UserId = request.UserId,
            GroupId = request.GroupId,
            GroupRoleId = role.Id,
            Status = RequestStatus.Pending.ToString(),
            RequestedAt = DateTime.UtcNow
        };

        await userGroupRepository.CreateAsync(userGroup, cancellationToken);
        logger.LogInformation("User {UserId} requested to join group {GroupId}", request.UserId, request.GroupId);
        
        await cache.IncrementVersionAsync("group-requests", request.GroupId, logger, cancellationToken);
        
        return ResultT<ResponseDto>.Success(
            new ResponseDto("Your request to join the group has been sent! The admin will review it shortly."));
    }
}