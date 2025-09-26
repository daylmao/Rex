using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.Groups.Commands.AproveRequest;

public record ApproveRequestCommand(
    Guid UserId,
    Guid GroupId
    ): ICommand<ResponseDto>;