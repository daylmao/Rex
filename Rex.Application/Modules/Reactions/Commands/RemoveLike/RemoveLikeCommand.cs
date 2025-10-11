using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Reactions.Commands.RemoveLike;

public record RemoveLikeCommand(
    Guid UserId,
    Guid PostId,
    ReactionTargetType ReactionTargetType
) : ICommand<ResponseDto>;