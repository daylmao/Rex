namespace Rex.Application.DTOs.User;

public record UserGroupRequestDto(
    string FirstName,
    string LastName,
    string ProfilePicture,
    string Status,
    TimeSpan TimeSinceRequested 
    );