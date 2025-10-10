using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Users.Commands.UpdatePasswordByEmail;

public record UpdatePasswordByEmailCommand(
    string Email
    ):ICommand<ResponseDto>;