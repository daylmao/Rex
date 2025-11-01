using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Commands.ManageRequest;

public record ManageRequestCommand(
    Guid UserId,
    Guid GroupId,
    ManageRequestStatus Status
    ): ICommand<ResponseDto>;