using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Groups.Commands.RequestToJoinGroup;

public record RequestToJoinGroupCommand(
    Guid UserId,
    Guid GroupId
    ):ICommand<ResponseDto>;