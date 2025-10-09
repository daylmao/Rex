using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Models;

namespace Rex.Application.Modules.Reactions.Commands.ToggleLikeCommand;

public class ToggleLikeCommandHandler(
    ILogger<ToggleLikeCommandHandler> logger,
    IReactionRepository reactionRepository,
    IUserRepository userRepository,
    IGroupRepository groupRepository,
    IPostRepository postRepository,
    IReactionNotifier reactionNotifier
) : ICommandHandler<ToggleLikeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("ToggleLikeCommand request is null.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "The request is invalid. Please check the details and try again."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found: {UserId}", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "User not found."));
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group not found: {GroupId}", request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Group not found."));
        }

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
        {
            logger.LogWarning("Post not found: {PostId}", request.PostId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Post not found."));
        }

        var reaction = await reactionRepository.HasLikedAsync(request.PostId, request.UserId, cancellationToken);

        if (reaction is null)
        {
            reaction = new Reaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                TargetId = request.PostId,
                TargetType = request.ReactionTargetType.ToString(),
                Like = true,
                CreatedAt = DateTime.UtcNow
            };

            await reactionRepository.CreateAsync(reaction, cancellationToken);

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
                await reactionNotifier.ReactionPostNotifier(notification, cancellationToken);
            }
            
            logger.LogInformation("User {UserId} liked post {PostId}.", request.UserId, request.PostId);
            return ResultT<ResponseDto>.Success(new("Impulse added successfully"));
        }

        reaction.Like = !reaction.Like;
        reaction.UpdatedAt = DateTime.UtcNow;

        await reactionRepository.UpdateAsync(reaction, cancellationToken);

        var toggleMessage = reaction.Like ? "Impulse added successfully" : "Impulse removed successfully";
        logger.LogInformation("User {UserId} toggled like on post {PostId}. New like status: {LikeStatus}",
            request.UserId, request.PostId, reaction.Like);

        return ResultT<ResponseDto>.Success(new(toggleMessage));
    }
}