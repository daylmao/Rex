using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.User.Commands.ResendCode;

public record ResendCodeCommand(
    string Email
    ): ICommand<ResponseDto>;