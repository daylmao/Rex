using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Enum;

namespace Rex.Application.Modules.Reactions.Commands.ToggleLikeCommand;

public record ToggleLikeCommand(
    Guid UserId,
    Guid TargetId,
    ReactionTargetType ReactionTargetType
    ): ICommand<ResponseDto>;