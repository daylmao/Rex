using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.User.Commands.RegisterUser;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    IFormFile ProfilePhoto,
    IFormFile? CoverPhoto,
    string? Biography,
    string Gender,
    DateTime Birthday
    ): ICommand<RegisterUserDto>;