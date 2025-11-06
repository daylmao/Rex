using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Users.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
    ): ICommand<TokenResponseDto>;