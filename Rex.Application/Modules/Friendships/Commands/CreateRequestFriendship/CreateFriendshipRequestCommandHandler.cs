using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Modules.Friendships.Commands.CreateRequestFriendship;

public class CreateFriendshipRequestCommandHandler(
    ILogger<CreateFriendshipRequestCommandHandler> logger,
    IFriendShipRepository friendShipRepository,
    IUserRepository userRepository,
    IFriendshipNotifier friendshipNotifier,
    IDistributedCache cache
) : ICommandHandler<CreateFriendshipRequestCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreateFriendshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Friendship request command is null");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Invalid friend request."));
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
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Requester account not found."));
        }

        if (targetUser is null)
        {
            logger.LogWarning("Target user not found: {UserId}", request.TargetUserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Target user not found."));
        }

        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.RequesterId, cancellationToken);
        if (!accountConfirmed)
        {
            logger.LogWarning("User {UserId} tried to send a friend request but the account is not confirmed.", request.RequesterId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You need to confirm your account before sending a friend request."));
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

            return ResultT<ResponseDto>.Failure(Error.Failure("409", "A friend request has already been sent to this user."));
        }

        var friendship = new FriendShip
        {
            RequesterId = request.RequesterId,
            TargetUserId = request.TargetUserId,
            Status = RequestStatus.Pending.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        await friendShipRepository.CreateAsync(friendship, cancellationToken);
        
        await cache.IncrementVersionAsync("friends", request.TargetUserId, logger, cancellationToken);
        logger.LogInformation("Cache invalidated for friends of UserId: {UserId}", request.TargetUserId);

        var metadata = new
        {
            RequesterId = requester.Id,
            RequesterName = $"{requester.FirstName} {requester.LastName}"
        };

        var notification = new Notification
        {
            Title = "New Friend Request",
            Description = $"{requester.FirstName} {requester.LastName} sent you a friend request.",
            UserId = requester.Id,
            RecipientType = TargetType.User.ToString(),         
            RecipientId = targetUser.Id,   
            MetadataJson = JsonSerializer.Serialize(metadata),
            CreatedAt = DateTime.UtcNow
        };

        await friendshipNotifier.SendFriendRequestNotification(notification, cancellationToken);
        
        logger.LogInformation("Friendship request {FriendshipId} sent from {RequesterId} to {TargetUserId}",
            friendship.Id, request.RequesterId, request.TargetUserId);

        return ResultT<ResponseDto>.Success(new ResponseDto("Friendship request sent successfully."));
    }
}