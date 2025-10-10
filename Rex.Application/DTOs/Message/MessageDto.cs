namespace Rex.Application.DTOs;

public record MessageDto(
    Guid MessageId,
    Guid ChatId,
    Guid SenderId,
    string Name,
    string ProfilePicture,
    string Description,
    DateTime CreatedAt,
    IEnumerable<FileDetailDto>? Files = null
    );