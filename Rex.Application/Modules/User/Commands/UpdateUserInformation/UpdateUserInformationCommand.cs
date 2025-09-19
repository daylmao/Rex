using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.User.Commands.UpdateUserInformation;

public record UpdateUserInformationCommand(
    Guid UserId,
    IFormFile ProfilePhoto,
    string Firstname,
    string Lastname,
    string Biography
    ): ICommand<ResponseDto>;