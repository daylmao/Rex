using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Commands.GroupUserModeration;

public class GroupUserModerationCommandHandler(
    ILogger<GroupUserModerationCommandHandler> logger,
    IUserGroupRepository userGroupRepository,
    IGroupRepository groupRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : ICommandHandler<GroupUserModerationCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(GroupUserModerationCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Tried to update status, but user {UserId} does not exist", request.MemberId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Oops! We couldn’t find that user."));
        }

        var group = await groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Tried to update status in group {GroupId}, but the group does not exist", request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Hmm… that group doesn’t exist."));
        }

        var member = await userGroupRepository.GetMemberAsync(request.MemberId, request.GroupId, cancellationToken);
        if (member is null)
        {
            logger.LogWarning("User {UserId} is not part of group {GroupId}", request.MemberId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Looks like this user isn’t part of the group."));
        }

        var alreadyBanned = await userGroupRepository.IsUserBannedAsync(request.MemberId, request.GroupId, cancellationToken);
        if (alreadyBanned)
        {
            logger.LogWarning("User {UserId} is already banned from group {GroupId}", request.MemberId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "This user is already banned from the group."));
        }

        if (member.Status == GroupUserModerationStatus.Removed.ToString())
        {
            logger.LogWarning("User {UserId} is already removed from group {GroupId}", request.MemberId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "This user has already been removed from the group."));
        }

        member.Status = request.Status.ToString();
        member.UpdatedAt = DateTime.UtcNow;
        await userGroupRepository.UpdateAsync(member, cancellationToken);
        
        await cache.IncrementVersionAsync("group-members", request.GroupId, logger, cancellationToken);
        await cache.IncrementVersionAsync("groups-user", request.MemberId, logger, cancellationToken);

        logger.LogInformation("User {UserId} status updated to {Status} in group {GroupId}", request.MemberId, request.Status, request.GroupId);
        return ResultT<ResponseDto>.Success(new ResponseDto($"Great! The user is now {request.Status}"));
    }
}
