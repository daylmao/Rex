namespace Rex.Application.DTOs.JWT;

public record GroupDetailsDto(
    Guid GroupId,
    string ProfilePicture,
    string CoverPicture,
    string Title,
    string Description,
    string Visibility,
    int MemberCount,
    bool IsJoined
    );