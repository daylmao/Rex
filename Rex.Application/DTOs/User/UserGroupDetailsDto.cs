namespace Rex.Application.DTOs;

public record UserGroupDetailsDto(
    Guid UserId,
    string Name,
    string Lastname,
    string Role,
    string ProfilePhoto
    );