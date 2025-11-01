using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;

namespace Rex.Application.Modules.Challenges.Commands.DeleteChallenge;

public record DeleteChallengeCommand(
    Guid ChallengeId,
    Guid GroupId,
    Guid UserId
    ): ICommand<ResponseDto>;