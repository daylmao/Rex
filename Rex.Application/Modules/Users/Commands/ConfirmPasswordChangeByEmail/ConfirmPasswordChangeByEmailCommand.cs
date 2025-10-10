using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Users.Commands.ConfirmPasswordChangeByEmail;

public record ConfirmPasswordChangeByEmailCommand(
    Guid UserId,
    string Code, 
    string NewPassword
    ): ICommand<ResponseDto>;