using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Challenges.CreateChallenge;

public record CreateChallengeCommand(
    Guid UserId,
    Guid GroupId,
    string Title,
    string Description,
    TimeSpan Duration,
    IFormFile CoverPhoto
    ): ICommand<ResponseDto>;