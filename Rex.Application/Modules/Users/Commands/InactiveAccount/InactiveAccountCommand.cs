using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.Users.Commands.InactiveAccount;

public record InactiveAccountCommand(
    Guid UserId
    ): ICommand<ResponseDto>;