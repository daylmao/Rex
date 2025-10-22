using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Groups.Commands.UpdateGroupRoleMember;

public class UpdateGroupRoleMemberCommandHandler(
    ILogger<UpdateGroupRoleMemberCommandHandler> logger,
    IUserGroupRepository userGroupRepository,
    IGroupRepository groupRepository,
    IUserRepository userRepository
) : ICommandHandler<UpdateGroupRoleMemberCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateGroupRoleMemberCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("UpdateGroupRoleMemberCommand received a null request.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! Something went wrong with the request. Please try again."));
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group with ID {GroupId} was not found.", request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "The group you're trying to update doesn't exist."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} was not found.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "The user was not found."));
        }

        var member = await userGroupRepository.GetMemberAsync(request.UserId, request.GroupId, cancellationToken);
        if (member is null)
        {
            logger.LogWarning("User {UserId} is not a member of group {GroupId}.", request.UserId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "This user is not part of the selected group."));
        }

        if (member.GroupRole is null)
        {
            logger.LogWarning("GroupRole is null for member {UserId} in group {GroupId}.", request.UserId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("500", "We couldn't retrieve the user's role in the group. Please try again later."));
        }

        member.GroupRole.Role = request.Role.ToString();

        await userGroupRepository.UpdateAsync(member, cancellationToken);

        logger.LogInformation("Successfully updated role of user {UserId} in group {GroupId} to {Role}.", request.UserId, request.GroupId, request.Role);

        return ResultT<ResponseDto>.Success(new ResponseDto("The user's role was successfully updated."));
    }
}
