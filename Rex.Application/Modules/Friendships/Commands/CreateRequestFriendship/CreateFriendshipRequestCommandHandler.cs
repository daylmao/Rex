using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Friendships.Commands;

public class CreateFriendshipRequestCommandHandler(
    ILogger<CreateFriendshipRequestCommandHandler> logger,
    IFriendShipRepository friendShipRepository,
    IUserRepository userRepository
) : ICommandHandler<CreateFriendshipRequestCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreateFriendshipRequestCommand createFriendshipRequest,
        CancellationToken cancellationToken)
    {
        if (createFriendshipRequest is null)
        {
            logger.LogWarning("Friendship request command is null");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! Something went wrong with your request."));
        }

        if (createFriendshipRequest.RequesterId == createFriendshipRequest.TargetUserId)
        {
            logger.LogWarning("User {UserId} tried to send a friend request to themselves", createFriendshipRequest.RequesterId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "You can't send a friend request to yourself"));
        }

        var requester = await userRepository.GetByIdAsync(createFriendshipRequest.RequesterId, cancellationToken);
        if (requester is null)
        {
            logger.LogWarning("User {UserId} not found", createFriendshipRequest.RequesterId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "We couldn't find your account"));
        }

        var targetUser = await userRepository.GetByIdAsync(createFriendshipRequest.TargetUserId, cancellationToken);
        if (targetUser is null)
        {
            logger.LogWarning("User {UserId} not found", createFriendshipRequest.TargetUserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "The user you want to add was not found"));
        }

        var friendshipExists =
            await friendShipRepository.FriendShipExistAsync(createFriendshipRequest.RequesterId, createFriendshipRequest.TargetUserId,
                cancellationToken);
        if (friendshipExists)
        {
            logger.LogWarning("Friendship already exists between {RequesterId} and {TargetUserId}", createFriendshipRequest.RequesterId,
                createFriendshipRequest.TargetUserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("409", "You are already friends with this user"));
        }

        var friendship = new Models.FriendShip
        {
            RequesterId = createFriendshipRequest.RequesterId,
            TargetUserId = createFriendshipRequest.TargetUserId,
            Status = RequestStatus.Pending.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        await friendShipRepository.CreateAsync(friendship, cancellationToken);
        logger.LogInformation("Friendship request sent from {RequesterId} to {TargetUserId}", createFriendshipRequest.RequesterId,
            createFriendshipRequest.TargetUserId);
        return ResultT<ResponseDto>.Success(new("Friendship request sent successfully"));
    }
}