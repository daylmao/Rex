using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Friendships.Commands.ManageFriendshipRequest;

public class ManageFriendshipRequestCommandHandler(
    ILogger<ManageFriendshipRequestCommandHandler> logger,
    IFriendShipRepository friendShipRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : ICommandHandler<ManageFriendshipRequestCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(ManageFriendshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received a null ManageFriendshipRequestCommand");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Invalid request. Please try again."));
        }

        if (request.RequesterId == request.TargetUserId)
        {
            logger.LogWarning("User {UserId} attempted to respond to their own friend request", request.RequesterId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "You cannot respond to your own friend request."));
        }

        var requester = await userRepository.GetByIdAsync(request.RequesterId, cancellationToken);
        var targetUser = await userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        
        if (requester is null)
        {
            logger.LogWarning("Requester user {UserId} not found", request.RequesterId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Your account could not be found."));
        }

        if (targetUser is null)
        {
            logger.LogWarning("Target user {UserId} not found", request.TargetUserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404",
                "The user you are trying to respond to could not be found."));
        }

        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.RequesterId, cancellationToken);
        if (!accountConfirmed)
        {
            logger.LogWarning("User with ID {UserId} tried to respond but the account is not confirmed.", request.RequesterId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You need to confirm your account first."));
        }
        
        var friendship = await friendShipRepository.GetFriendShipInPendingAsync(request.RequesterId,
            request.TargetUserId,
            cancellationToken
        );

        if (friendship is null)
        {
            logger.LogWarning(
                "No pending friendship request found between requester {RequesterId} and target {TargetUserId}",
                request.RequesterId, request.TargetUserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404",
                "No pending friend request was found between these users."));
        }

        if (friendship.Status != RequestStatus.Pending.ToString())
        {
            logger.LogWarning("Friendship request between {RequesterId} and {TargetUserId} has already been processed",
                request.RequesterId, request.TargetUserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("409",
                "This friend request has already been responded to."));
        }

        friendship.Status = request.Status.ToString();
        friendship.UpdatedAt = DateTime.UtcNow;
        await friendShipRepository.UpdateAsync(friendship, cancellationToken);

        logger.LogInformation(
            "Friendship request from {RequesterId} to {TargetUserId} was {Status}",
            request.RequesterId,
            request.TargetUserId,
            request.Status
        );

        await cache.IncrementVersionAsync("friends", request.RequesterId, logger, cancellationToken);
        await cache.IncrementVersionAsync("friends", request.TargetUserId, logger, cancellationToken);
        logger.LogInformation("Cache invalidated for friends of UserIds: {RequesterId}, {TargetUserId}", request.RequesterId, request.TargetUserId);

        var message = request.Status == ManageRequestStatus.Accepted
            ? "Friend request accepted successfully."
            : "Friend request rejected successfully.";

        return ResultT<ResponseDto>.Success(new ResponseDto(message));
    }
}
