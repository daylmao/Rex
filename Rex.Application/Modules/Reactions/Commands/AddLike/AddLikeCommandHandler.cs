using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.DTOs.Reaction;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Modules.Reactions.Commands.AddLike;

public class AddLikeCommandHandler(
    ILogger<AddLikeCommandHandler> logger,
    IReactionRepository reactionRepository,
    IUserRepository userRepository,
    IGroupRepository groupRepository,
    IUserGroupRepository userGroupRepository,
    IPostRepository postRepository,
    IReactionNotifier reactionNotifier
) : ICommandHandler<AddLikeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(AddLikeCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing like for PostId: {PostId} by UserId: {UserId}", request.PostId, request.UserId);

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found: {UserId}", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "We couldn't find your account."));
        }

        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.UserId, cancellationToken);
        if (!accountConfirmed)
        {
            logger.LogWarning("User {UserId} tried to react but account is not confirmed.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "Please confirm your account to give an impulse."));
        }

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
        {
            logger.LogWarning("Post not found: {PostId}", request.PostId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Oops! The post doesn't exist."));
        }

        var group = await groupRepository.GetByIdAsync(post.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group not found for PostId {PostId}", request.PostId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "The group for this post doesn't exist."));
        }

        var isMember = await userGroupRepository.IsUserInGroupAsync(
            request.UserId, post.GroupId, RequestStatus.Accepted, cancellationToken);
        if (!isMember)
        {
            logger.LogWarning("User {UserId} tried to react but is not a member of group {GroupId}", request.UserId, post.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You need to join the group to give an impulse."));
        }

        var existingReaction = await reactionRepository.HasLikedAsync(request.PostId, request.UserId, cancellationToken);

        if (existingReaction is not null && existingReaction.Like)
        {
            logger.LogInformation("User {UserId} already liked PostId {PostId}", request.UserId, request.PostId);
            return ResultT<ResponseDto>.Success(new ResponseDto("You already gave an impulse to this post."));
        }

        if (existingReaction is not null)
        {
            existingReaction.Like = true;
            existingReaction.UpdatedAt = DateTime.UtcNow;
            await reactionRepository.UpdateAsync(existingReaction, cancellationToken);

            logger.LogInformation("User {UserId} restored like on PostId {PostId}", request.UserId, request.PostId);

            var totalLikes = await reactionRepository.CountLikesAsync(request.PostId, request.ReactionTargetType, cancellationToken);
            await reactionNotifier.LikeChangedNotificationAsync(new LikeChangedDto(
                PostId: request.PostId,
                TotalLikes: totalLikes,
                UserId: request.UserId,
                Liked: true
            ), cancellationToken);

            return ResultT<ResponseDto>.Success(new ResponseDto("You just gave an impulse!"));
        }

        var reaction = new Reaction
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            TargetId = request.PostId,
            TargetType = request.ReactionTargetType.ToString(),
            Like = true,
            CreatedAt = DateTime.UtcNow
        };

        await reactionRepository.CreateAsync(reaction, cancellationToken);
        logger.LogInformation("User {UserId} liked PostId {PostId}", request.UserId, request.PostId);

        if (post.UserId != request.UserId)
        {
            var metadata = new
            {
                GroupId = group.Id,
                PostId = post.Id,
                PostTitle = post.Title,
                LikedBy = $"{user.FirstName} {user.LastName}"
            };

            var notification = new Notification
            {
                Title = "New Impulse!",
                Description = $"{user.FirstName} {user.LastName} just gave an impulse to your post in '{group.Title}'",
                UserId = user.Id,
                RecipientType = TargetType.User.ToString(),
                RecipientId = post.UserId,
                MetadataJson = JsonSerializer.Serialize(metadata),
                Read = false,
                CreatedAt = DateTime.UtcNow
            };

            await reactionNotifier.ReactionPostNotificationAsync(notification, cancellationToken);
            logger.LogInformation("Notification sent to UserId {RecipientId} for PostId {PostId}", post.UserId, request.PostId);
        }

        var totalLikesFinal = await reactionRepository.CountLikesAsync(request.PostId, request.ReactionTargetType, cancellationToken);
        await reactionNotifier.LikeChangedNotificationAsync(new LikeChangedDto(
            PostId: request.PostId,
            TotalLikes: totalLikesFinal,
            UserId: request.UserId,
            Liked: true
        ), cancellationToken);

        logger.LogInformation("Like process completed for PostId {PostId} by UserId {UserId}", request.PostId, request.UserId);
        return ResultT<ResponseDto>.Success(new ResponseDto("You just gave an impulse!"));
    }
}