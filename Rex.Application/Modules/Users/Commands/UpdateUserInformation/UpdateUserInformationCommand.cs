using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Users.Commands.UpdateUserInformation;

public record UpdateUserInformationCommand(
    Guid UserId,
    IFormFile? ProfilePhoto,
    string? Firstname,
    string? Lastname,
    string? Biography
    ): ICommand<ResponseDto>;