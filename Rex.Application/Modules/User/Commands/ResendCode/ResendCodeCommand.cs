using Rex.Application.Abstractions.Messages;

namespace Rex.Application.Modules.User.Commands.ResendCode;

public record ResendCodeCommand(
    string Email
    ): ICommand<string>;