using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Groups.Commands.UpdateGroup;

public record UpdateGroupCommand(
    Guid GroupId,
    IFormFile? ProfilePhoto,
    IFormFile? CoverPhoto,
    string? Title,
    string? Description,
    GroupVisibility? Visibility
    ): ICommand<ResponseDto>;