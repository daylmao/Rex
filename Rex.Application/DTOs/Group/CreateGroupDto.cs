using Microsoft.AspNetCore.Http;
using Rex.Enum;

namespace Rex.Application.DTOs.Group;

public record CreateGroupDto(
    IFormFile ProfilePhoto,
    IFormFile? CoverPhoto,
    string Title,
    string Description,
    GroupVisibility Visibility
    );