using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Users.Commands.UpdateUsername;

public record UpdateUsernameCommand(
    Guid UserId,
    string Username
    ): ICommand<ResponseDto>;