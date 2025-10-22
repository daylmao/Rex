namespace Rex.Application.DTOs.User;

public record UpdatePasswordDto(
    string CurrentPassword,
    string NewPassword
    );