using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.DTOs.Reaction;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Reactions.Commands.RemoveLike;

public class RemoveLikeCommandHandler(
    ILogger<RemoveLikeCommandHandler> logger,
    IReactionRepository reactionRepository,
    IPostRepository postRepository,
    IReactionNotifier reactionNotifier
) : ICommandHandler<RemoveLikeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(RemoveLikeCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Post not found."));

        var reaction = await reactionRepository.HasLikedAsync(request.PostId, request.UserId, cancellationToken);
        
        if (reaction is null || !reaction.Like)
        {
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Like not found."));
        }

        reaction.Like = false;
        reaction.UpdatedAt = DateTime.UtcNow;
        await reactionRepository.UpdateAsync(reaction, cancellationToken);

        logger.LogInformation("User {UserId} removed like from post {PostId}", request.UserId, request.PostId);

        var totalLikes = await reactionRepository.CountLikesAsync(request.PostId, request.ReactionTargetType, cancellationToken);
        var likeChangedDto = new LikeChangedDto(
            PostId: request.PostId,
            TotalLikes: totalLikes,
            UserId: request.UserId,
            Liked: false
        );
        await reactionNotifier.LikeChangedNotificationAsync(likeChangedDto, cancellationToken);

        return ResultT<ResponseDto>.Success(new("Impulse removed successfully"));
    }
}