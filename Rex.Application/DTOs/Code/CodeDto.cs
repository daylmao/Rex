namespace Rex.Application.DTOs.Code;

public record CodeDto
(
    Guid UserId,
    Guid CodeId,
    string Code,
    bool IsUsed,
    DateTime Expiration
);