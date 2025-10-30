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
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "User not found."));
        
        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.UserId, cancellationToken);
        if (!accountConfirmed)
        {
            logger.LogWarning("User with ID {UserId} tried to create a group but the account is not confirmed.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You need to confirm your account before creating a group."));
        }

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Post not found."));

        var group = await groupRepository.GetByIdAsync(post.GroupId, cancellationToken);
        if (group is null)
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Group not found."));

        var isMember = await userGroupRepository.IsUserInGroupAsync(request.UserId, post.GroupId,
            RequestStatus.Accepted, cancellationToken);
        if (!isMember)
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You don't have access to this group."));

        var existingReaction =
            await reactionRepository.HasLikedAsync(request.PostId, request.UserId, cancellationToken);

        if (existingReaction is not null && existingReaction.Like)
            return ResultT<ResponseDto>.Success(new("Impulse already added"));

        if (existingReaction is not null)
        {
            existingReaction.Like = true;
            existingReaction.UpdatedAt = DateTime.UtcNow;
            await reactionRepository.UpdateAsync(existingReaction, cancellationToken);

            logger.LogInformation("User {UserId} restored like on post {PostId}", request.UserId, request.PostId);

            var restoredTotalLikes =
                await reactionRepository.CountLikesAsync(request.PostId, request.ReactionTargetType, cancellationToken);
            var restoredLikeChangedDto = new LikeChangedDto(
                PostId: request.PostId,
                TotalLikes: restoredTotalLikes,
                UserId: request.UserId,
                Liked: true
            );
            await reactionNotifier.LikeChangedNotificationAsync(restoredLikeChangedDto, cancellationToken);

            return ResultT<ResponseDto>.Success(new("Impulse added successfully"));
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
        logger.LogInformation("User {UserId} liked post {PostId}", request.UserId, request.PostId);

        if (post.UserId != request.UserId)
        {
            var notification = new Notification
            {
                Title = "New Impulse on Your Post",
                Description = $"{user.FirstName} {user.LastName} gave an impulse to your post in '{group.Title}'",
                UserId = user.Id,
                RecipientId = post.UserId,
                Read = false,
                CreatedAt = DateTime.UtcNow
            };
            await reactionNotifier.ReactionPostNotificationAsync(notification, cancellationToken);
        }

        var totalLikes =
            await reactionRepository.CountLikesAsync(request.PostId, request.ReactionTargetType, cancellationToken);

        var likeChangedDto = new LikeChangedDto(
            PostId: request.PostId,
            TotalLikes: totalLikes,
            UserId: request.UserId,
            Liked: true
        );
        await reactionNotifier.LikeChangedNotificationAsync(likeChangedDto, cancellationToken);

        return ResultT<ResponseDto>.Success(new("Impulse added successfully"));
    }
}