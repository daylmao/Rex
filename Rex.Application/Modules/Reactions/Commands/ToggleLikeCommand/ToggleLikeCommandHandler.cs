using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Models;

namespace Rex.Application.Modules.Reactions.Commands.ToggleLikeCommand;

public class ToggleLikeCommandHandler(
    ILogger<ToggleLikeCommandHandler> logger,
    IReactionRepository reactionRepository
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

        var reaction = await reactionRepository.HasLikedAsync(
            request.TargetId, request.UserId, request.ReactionTargetType, cancellationToken);

        if (reaction == null)
        {
            reaction = new Reaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                TargetId = request.TargetId,
                TargetType = request.ReactionTargetType.ToString(),
                Like = true,
                CreatedAt = DateTime.UtcNow
            };

            await reactionRepository.CreateAsync(reaction, cancellationToken);

            return ResultT<ResponseDto>.Success(new ResponseDto("Like added successfully"));
        }

        reaction.Like = !reaction.Like;
        reaction.UpdatedAt = DateTime.UtcNow;
        await reactionRepository.UpdateAsync(reaction, cancellationToken);

        var message = reaction.Like ? "Like added successfully" : "Like removed successfully";
        return ResultT<ResponseDto>.Success(new ResponseDto(message));
    }
}