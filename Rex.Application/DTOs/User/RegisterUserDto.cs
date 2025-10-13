namespace Rex.Application.DTOs.User;

public record RegisterUserDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string ProfilePhoto,
    string? CoverPhoto,
    string? Biography,
    string Gender,
    DateTime Birthday
    );