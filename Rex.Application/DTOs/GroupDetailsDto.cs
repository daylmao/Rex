namespace Rex.Application.DTOs;

public record GroupDetailsDto(
    string ProfilePicture,
    string CoverPicture,
    string Title,
    string Description,
    string Visibility,
    int MemberCount,
    bool IsJoined
    );