namespace Rex.Application.DTOs.User;

public record UserGroupDetailsDto(
    Guid UserId,
    string Name,
    string Lastname,
    string Role,
    string ProfilePhoto
    );