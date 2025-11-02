namespace Rex.Application.DTOs.User;

public record UserGroupRequestDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string ProfilePicture,
    string Status,
    TimeSpan TimeSinceRequested 
    );