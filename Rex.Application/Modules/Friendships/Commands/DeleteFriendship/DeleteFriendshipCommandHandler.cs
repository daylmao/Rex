using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Friendships.Commands.DeleteFriendship;

public class DeleteFriendshipCommandHandler(
    ILogger<DeleteFriendshipCommandHandler> logger,
    IChatRepository chatRepository,
    IFriendShipRepository friendShipRepository,
    IDistributedCache cache
) : ICommandHandler<DeleteFriendshipCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(DeleteFriendshipCommand request, CancellationToken cancellationToken)
    {
        var friendship = await friendShipRepository.GetFriendShipBetweenUsersAsync(
            request.RequesterId, request.TargetUserId, cancellationToken);

        if (friendship is null)
        {
            logger.LogWarning("Friendship not found between users {RequesterId} and {TargetUserId}.",
                request.RequesterId, request.TargetUserId);

            return ResultT<ResponseDto>.Failure(Error.NotFound("404",
                "Friendship between these users does not exist."));
        }

        if (friendship.RequesterId != request.RequesterId && friendship.TargetUserId != request.RequesterId)
        {
            logger.LogWarning("User {UserId} attempted to delete a friendship they are not part of.",
                request.RequesterId);

            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You are not part of this friendship."));
        }

        var chat = await chatRepository.GetOneToOneChat(request.RequesterId, request.TargetUserId, cancellationToken);
        if (chat is null)
        {
            logger.LogWarning("No chat found for users {RequesterId} and {TargetUserId}.",
                request.RequesterId, request.TargetUserId);

            return ResultT<ResponseDto>.Failure(Error.NotFound("404",
                "No chat exists between these users."));
        }

        if (chat.Deleted || friendship.Deleted)
        {
            logger.LogWarning(
                "Attempt to delete friendship or chat between users {RequesterId} and {TargetUserId} which is already deleted.",
                request.RequesterId, request.TargetUserId);

            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "This friendship and its chat have already been deleted."));
        }

        chat.Deleted = true;
        chat.DeletedAt = DateTime.UtcNow;

        friendship.Deleted = true;
        friendship.DeletedAt = DateTime.UtcNow;

        await friendShipRepository.UpdateAsync(friendship, cancellationToken);
        await chatRepository.UpdateAsync(chat, cancellationToken);

        await cache.IncrementVersionAsync("friends", request.RequesterId, logger, cancellationToken);
        await cache.IncrementVersionAsync("friends", request.TargetUserId, logger, cancellationToken);
        logger.LogInformation("Cache invalidated for friends of UserIds: {RequesterId}, {TargetUserId}", request.RequesterId, request.TargetUserId);

        logger.LogInformation(
            "Friendship and associated chat between users {RequesterId} and {TargetUserId} marked as deleted.",
            request.RequesterId, request.TargetUserId);

        return ResultT<ResponseDto>.Success(
            new ResponseDto("Friendship successfully deleted and chat deactivated."));
    }
}