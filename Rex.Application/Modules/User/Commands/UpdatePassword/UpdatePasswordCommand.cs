using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.User.Commands.UpdatePassword;

public record UpdatePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
    ): ICommand<AnswerDto>;