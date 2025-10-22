using Microsoft.AspNetCore.Http;
using Rex.Enum;

namespace Rex.Application.DTOs.Group;

public record CreateGroupDto(
    Guid GroupId,
    Guid? ChallengeId,
    string Title,
    string Description,
    List<IFormFile>? Files = null
    );