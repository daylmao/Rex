using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Friendships.Commands.CreateRequestFriendship;

public record CreateFriendshipRequestCommand(
    Guid RequesterId,
    Guid TargetUserId
    ):ICommand<ResponseDto>;