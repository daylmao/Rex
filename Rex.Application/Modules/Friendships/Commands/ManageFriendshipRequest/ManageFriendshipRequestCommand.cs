using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Friendships.Commands.ManageFriendshipRequest;

public record ManageFriendshipRequestCommand(
    Guid RequesterId,
    Guid TargetUserId,
    RequestStatus Status
    ): ICommand<ResponseDto>;