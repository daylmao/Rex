using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Modules.Friendships.Commands;

public class CreateFriendshipRequestCommandHandler(
    ILogger<CreateFriendshipRequestCommandHandler> logger,
    IFriendShipRepository friendShipRepository,
    IUserRepository userRepository,
    IFriendshipNotifier friendshipNotifier
) : ICommandHandler<CreateFriendshipRequestCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreateFriendshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Friendship request command is null");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! Something went wrong with your request."));
        }

        if (request.RequesterId == request.TargetUserId)
        {
            logger.LogWarning("User {UserId} tried to send a friend request to themselves", request.RequesterId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "You can't send a friend request to yourself."));
        }

        var requester = await userRepository.GetByIdAsync(request.RequesterId, cancellationToken);
        var targetUser = await userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        
        if (requester is null)
        {
            logger.LogWarning("Requester user not found: {UserId}", request.RequesterId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "We couldn't find your account."));
        }

        if (targetUser is null)
        {
            logger.LogWarning("Target user not found: {UserId}", request.TargetUserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "The user you want to add was not found."));
        }

        var friendshipExists = await friendShipRepository.FriendShipExistAsync(
            request.RequesterId,
            request.TargetUserId,
            cancellationToken
        );

        if (friendshipExists)
        {
            logger.LogWarning("Friendship already exists between {RequesterId} and {TargetUserId}",
                request.RequesterId, request.TargetUserId);

            return ResultT<ResponseDto>.Failure(Error.Failure("409", "You are already have sent a request to this user."));
        }

        var friendship = new FriendShip
        {
            RequesterId = request.RequesterId,
            TargetUserId = request.TargetUserId,
            Status = RequestStatus.Pending.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        await friendShipRepository.CreateAsync(friendship, cancellationToken);

        var notification = new Notification
        {
            Title = "New Friend Request",
            Description = $"{requester.FirstName} {requester.LastName} sent you a friend request.",
            UserId = requester.Id,
            RecipientId = targetUser.Id,
            Read = false,
            CreatedAt = DateTime.UtcNow
        };

        await friendshipNotifier.SendFriendRequestNotification(notification, cancellationToken);

        logger.LogInformation("Friendship request sent from {RequesterId} to {TargetUserId}",
            request.RequesterId, request.TargetUserId);

        return ResultT<ResponseDto>.Success(new("Friendship request sent successfully."));
    }
}