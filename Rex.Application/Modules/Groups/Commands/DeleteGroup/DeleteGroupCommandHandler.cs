using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Commands.DeleteGroup;

public class DeleteGroupCommandHandler(
    ILogger<DeleteGroupCommandHandler> logger,
    IGroupRepository groupRepository,
    IUserRepository userRepository,
    IUserGroupRepository userGroupRepository
) : ICommandHandler<DeleteGroupCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("We couldn't find the group with ID {GroupId}.", request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Oops! We couldn't find the group you're looking for."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("We couldn't find the user with ID {UserId}.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Oops! We couldn't find the user."));
        }

        var userInGroup = await userGroupRepository.IsUserInGroupAsync(request.UserId, request.GroupId,
            RequestStatus.Accepted, cancellationToken);
        if (!userInGroup)
        {
            logger.LogWarning("User with ID {UserId} is not a member of the group with ID {GroupId}.", request.UserId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "It seems you're not part of this group."));
        }

        var isUserGroupLeader =
            await userGroupRepository.GetMemberAsync(request.UserId, request.GroupId, cancellationToken);
        
        if (isUserGroupLeader.GroupRole.Role == GroupRole.Leader.ToString())
        {
            logger.LogWarning("User with ID {UserId} does not have permission to delete group {GroupId}.", request.UserId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "Only the group leader can delete the group."));
        }

        logger.LogInformation("Deleting the group with ID {GroupId}.", request.GroupId);
        await groupRepository.DeleteAsync(group, cancellationToken);

        return ResultT<ResponseDto>.Success(new ResponseDto("The group has been successfully deleted!"));
    }
}