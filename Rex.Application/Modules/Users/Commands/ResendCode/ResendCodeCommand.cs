using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Users.Commands.ResendCode;

public record ResendCodeCommand(
    string Email
    ): ICommand<ResponseDto>;