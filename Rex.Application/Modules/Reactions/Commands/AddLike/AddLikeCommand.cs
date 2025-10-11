using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Reactions.Commands.AddLike;

public record AddLikeCommand(
    Guid UserId,
    Guid PostId,
    ReactionTargetType ReactionTargetType
) : ICommand<ResponseDto>;