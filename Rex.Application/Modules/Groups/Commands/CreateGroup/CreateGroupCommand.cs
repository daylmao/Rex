using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Commands.CreateGroup;

public record CreateGroupCommand(
    Guid UserId,
    IFormFile ProfilePhoto,
    IFormFile? CoverPhoto,
    string Title,
    string Description,
    GroupVisibility Visibility
    ): ICommand<ResponseDto>;