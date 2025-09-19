namespace Rex.Application.DTOs;

public record UserProfileDto(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string ProfilePhoto,
    string? CoverPhoto,
    DateTime Birthday,
    DateTime CreatedAt,
    string? Biography,
    string Gender,
    DateTime? LastLoginAt,
    int GroupsCount,
    int LikesCount,
    int ChallengesCount
    );