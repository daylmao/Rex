using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Users.Commands.UpdateEmail;

public record UpdateEmailCommand(
    Guid UserId,
    string Email,
    string NewEmail
    ): ICommand<ResponseDto>;