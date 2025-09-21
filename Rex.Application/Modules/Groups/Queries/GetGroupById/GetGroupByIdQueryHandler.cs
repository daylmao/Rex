using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Queries.GetGroupById;

public class GetGroupByIdQueryHandler(
    ILogger<GetGroupByIdQueryHandler> logger,
    IGroupRepository groupService,
    IUserGroupRepository userGroupRepository,
    IUserRepository userRepository
) : IQueryHandler<GetGroupByIdQuery, GroupDetailsDto>
{
    public async Task<ResultT<GroupDetailsDto>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received a null request for GetGroupByIdQuery.");
            return ResultT<GroupDetailsDto>.Failure(
                Error.Failure("400", "Oops! No request data was provided."));
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Could not find a user with ID {UserId}.", request.UserId);
            return ResultT<GroupDetailsDto>.Failure(
                Error.NotFound("404", "We couldn't find the user making the request."));
        }

        var group = await groupService.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Could not find a group with ID {GroupId}.", request.GroupId);
            return ResultT<GroupDetailsDto>.Failure(
                Error.NotFound("404", "We couldn't find the group you're looking for."));
        }

        var isUserBanned =
            await userGroupRepository.IsUserBannedAsync(request.UserId, request.GroupId, cancellationToken);
        
        if (isUserBanned)
        {
            logger.LogWarning("User {UserId} is banned from group {GroupId}.", request.UserId, request.GroupId);
            return ResultT<GroupDetailsDto>.Failure(
                Error.Failure("403", "You can't access this group because you're banned."));
        }

        var memberCount = await userGroupRepository.GetUserGroupCountAsync(request.GroupId, RequestStatus.Accepted,
                cancellationToken);

        var isGroupPrivate = await userGroupRepository.IsGroupPrivateAsync(request.GroupId, cancellationToken);
        var isUserInGroup = await userGroupRepository.IsUserInGroupAsync(
            request.UserId, request.GroupId, RequestStatus.Accepted, cancellationToken);

        if (isGroupPrivate && !isUserInGroup)
        {
            logger.LogWarning("User {UserId} tried to view private group {GroupId} without membership.", request.UserId,
                request.GroupId);
            return ResultT<GroupDetailsDto>.Failure(
                Error.Failure("403", "This group is private, and you’re not a member."));
        }

        logger.LogInformation("Returning details for group {GroupId} to user {UserId}. Membership: {IsJoined}",
            request.GroupId, request.UserId, isUserInGroup);

        return ResultT<GroupDetailsDto>.Success(new GroupDetailsDto(
            ProfilePicture: group.ProfilePhoto,
            CoverPicture: group.CoverPhoto ?? string.Empty,
            Title: group.Title,
            Description: group.Description,
            Visibility: group.Visibility,
            MemberCount: memberCount,
            IsJoined: isUserInGroup
        ));
    }
}