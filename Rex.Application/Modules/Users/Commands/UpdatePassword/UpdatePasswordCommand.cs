using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Users.Commands.UpdatePassword;

public record UpdatePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
    ): ICommand<ResponseDto>;