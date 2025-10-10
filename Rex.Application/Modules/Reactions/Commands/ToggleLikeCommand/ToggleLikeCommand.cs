using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Reactions.Commands.ToggleLikeCommand;

public record ToggleLikeCommand(
    Guid UserId,
    Guid PostId,
    Guid GroupId,
    ReactionTargetType ReactionTargetType
    ): ICommand<ResponseDto>;