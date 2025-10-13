namespace Rex.Application.DTOs.Friendship;

public record FriendshipRequestDto(
    Guid FriendshipId,
    Guid UserId,
    string FullName,
    string Status,
    string ProfilePictureUrl,
    DateTime RequestedAt
    );