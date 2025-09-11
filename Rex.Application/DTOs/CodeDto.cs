namespace Rex.Application.DTOs;

public record CodeDto
(
    Guid UserId,
    Guid CodeId,
    string Code,
    bool IsUsed,
    DateTime Expiration
);