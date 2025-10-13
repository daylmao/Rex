using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Challenges.Commands.DeleteChallenge;

public class DeleteChallengeCommandHandler(
    ILogger<DeleteChallengeCommandHandler> logger,
    IGroupRepository groupRepository,
    IChallengeRepository challengeRepository,
    IUserRepository userRepository,
    IUserGroupRepository userGroupRepository
) : ICommandHandler<DeleteChallengeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(DeleteChallengeCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound(
                "404",
                "We couldn't find your account. Please make sure you're logged in."
            ));
        }

        var challenge = await challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);
        if (challenge is null)
        {
            logger.LogWarning("Challenge with ID {ChallengeId} not found.", request.ChallengeId);
            return ResultT<ResponseDto>.Failure(Error.NotFound(
                "404",
                "The challenge you're trying to delete doesn't exist or was already removed."
            ));
        }

        var groupId = challenge.GroupId;

        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group with ID {GroupId} not found.", groupId);
            return ResultT<ResponseDto>.Failure(Error.NotFound(
                "404",
                "The group you're trying to access couldn't be found. Please check if you still belong to it."
            ));
        }

        var isMember = await userGroupRepository.IsUserInGroupAsync(request.UserId, groupId, RequestStatus.Accepted, cancellationToken);
        if (!isMember)
        {
            logger.LogWarning("User {UserId} is not a member of group {GroupId}.", request.UserId, groupId);
            return ResultT<ResponseDto>.Failure(Error.Failure(
                "403",
                "You are not a member of this group or your membership hasn't been approved yet."
            ));
        }

        var userGroup = await userGroupRepository.GetMemberAsync(request.UserId, groupId, cancellationToken);
        if (userGroup is null)
        {
            logger.LogWarning("Membership not found for user {UserId} in group {GroupId}.", request.UserId, groupId);
            return ResultT<ResponseDto>.Failure(Error.NotFound(
                "404",
                "We couldn't verify your membership in this group. Please try refreshing or contact an admin."
            ));
        }

        if (userGroup.GroupRole.Role.Equals(GroupRole.Member.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("User {UserId} lacks permission to delete challenges in group {GroupId}.", request.UserId, groupId);
            return ResultT<ResponseDto>.Failure(Error.Failure(
                "403",
                "Only group admins can delete challenges."
            ));
        }

        await challengeRepository.DeleteAsync(challenge, cancellationToken);
        logger.LogInformation("Challenge {ChallengeId} deleted by user {UserId} from group {GroupId}.",
            request.ChallengeId, request.UserId, groupId);

        return ResultT<ResponseDto>.Success(new ResponseDto("Challenge was successfully deleted."));
    }
}
