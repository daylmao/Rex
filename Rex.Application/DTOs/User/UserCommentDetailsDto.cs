namespace Rex.Application.DTOs.User;

public record UserCommentDetailsDto(
    Guid UserId,
    string Name,
    string Lastname,
    string ProfilePhoto
    );