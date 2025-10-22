using Microsoft.AspNetCore.Http;

namespace Rex.Application.DTOs.Post;

public record CreatePostDto(
    Guid GroupId,
    Guid? ChallengeId,
    string Title,
    string Description,
    List<IFormFile>? Files = null
    );