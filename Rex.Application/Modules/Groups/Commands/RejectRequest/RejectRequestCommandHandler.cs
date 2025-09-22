using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Commands.RejectRequest;

public class RejectRequestCommandHandler(
    ILogger<RejectRequestCommandHandler> logger,
    IGroupRepository groupRepository,
    IUserRepository userRepository,
    IUserGroupRepository userGroupRepository
) : ICommandHandler<RejectRequestCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(RejectRequestCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("RejectRequestCommand received is null");
            return ResultT<ResponseDto>.Failure(
                Error.Failure("400", "Oops! We didn't receive a valid request to reject."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found", request.UserId);
            return ResultT<ResponseDto>.Failure(
                Error.NotFound("404", "We couldn't find the user whose request you want to reject."));
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group with ID {GroupId} not found", request.GroupId);
            return ResultT<ResponseDto>.Failure(
                Error.NotFound("404", "The group for which you want to reject the request does not exist."));
        }

        var requestExists =
            await userGroupRepository.GetGroupRequestAsync(request.UserId, request.GroupId, cancellationToken);
        if (requestExists is null)
        {
            logger.LogWarning("No pending request found for user {UserId} in group {GroupId}", request.UserId,
                request.GroupId);
            return ResultT<ResponseDto>.Failure(
                Error.NotFound("404", "There is no pending join request from this user to reject."));
        }
        
        if (requestExists.Status != RequestStatus.Pending.ToString())
        {
            logger.LogWarning("Cannot reject request for user {UserId} in group {GroupId} because it is {Status}", 
                request.UserId, request.GroupId, requestExists.Status);

            return ResultT<ResponseDto>.Failure(
                Error.Conflict("409", $"Cannot reject this request because it is already {requestExists.Status}."));
        }

        requestExists.Status = RequestStatus.Rejected.ToString();
        await userGroupRepository.UpdateAsync(requestExists, cancellationToken);

        logger.LogInformation("Join request from user {UserId} for group {GroupId} rejected successfully",
            request.UserId, request.GroupId);
        return ResultT<ResponseDto>.Success(
            new ResponseDto("The user's request to join the group has been rejected successfully."));
    }
}
