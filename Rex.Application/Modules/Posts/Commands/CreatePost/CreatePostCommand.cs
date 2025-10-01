using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.Posts.Commands;

public record CreatePostCommand(
    Guid GroupId,
    Guid UserId,
    Guid? ChallengeId,
    string Title,
    string Description,
    List<IFormFile> Files
) : ICommand<ResponseDto>;