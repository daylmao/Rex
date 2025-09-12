using Rex.Application.Abstractions.Messages;

namespace Rex.Application.Modules.User.Commands.ConfirmAccount;

public record ConfirmAccountCommand(
    Guid UserId,
    string Code
    ): ICommand<string>;