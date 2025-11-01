using Microsoft.AspNetCore.Http;
using Rex.Enum;

namespace Rex.Application.DTOs.Group;

public record UpdateGroupDto(
    IFormFile? ProfilePhoto,
    IFormFile? CoverPhoto,
    string? Title,
    string? Description,
    GroupVisibility? Visibility
    );