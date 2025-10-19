using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Friendships.Commands.DeleteFriendship;

public record DeleteFriendshipCommand(
    Guid RequesterId,
    Guid TargetUserId
    ): ICommand<ResponseDto>;