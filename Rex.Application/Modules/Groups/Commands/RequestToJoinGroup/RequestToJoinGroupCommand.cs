using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.Groups.Commands.RequestToJoinGroupCommand;

public record RequestToJoinGroupCommand(
    Guid UserId,
    Guid GroupId
    ):ICommand<ResponseDto>;