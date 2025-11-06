using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Groups.Commands.ApproveRequest;

public record ApproveRequestCommand(
    Guid UserId,
    Guid GroupId
    ): ICommand<ResponseDto>;