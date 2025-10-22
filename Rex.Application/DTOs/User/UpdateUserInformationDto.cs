using Microsoft.AspNetCore.Http;

namespace Rex.Application.DTOs.User;

public record UpdateUserInformationDto(
    IFormFile? ProfilePhoto,
    string? Firstname,
    string? Lastname,
    string? Biography
    );