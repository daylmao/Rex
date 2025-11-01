using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Enum;

namespace Rex.Application.Modules.Challenges.Commands.UpdateChallenge;

public record UpdateChallengeCommand(
    Guid GroupId,
    Guid ChallengeId,
    string Title,
    string Description,
    TimeSpan Duration,
    ChallengeStatus Status
    ): ICommand<ResponseDto>;