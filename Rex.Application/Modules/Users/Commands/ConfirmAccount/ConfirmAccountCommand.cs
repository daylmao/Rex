using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.User.Commands.ConfirmAccount;

public record ConfirmAccountCommand(
    Guid UserId,
    string Code
    ): ICommand<ResponseDto>;