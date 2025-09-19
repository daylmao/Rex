using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.User.Commands.ConfirmEmailChange;

public record ConfirmEmailChangeCommand(
    Guid UserId,
    string Code
    ):ICommand<ResponseDto>;