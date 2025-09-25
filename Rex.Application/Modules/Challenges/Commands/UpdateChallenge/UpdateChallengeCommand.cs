using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.Challenges.Commands.UpdateChallenge;

public record UpdateChallengeCommand(
    Guid ChallengeId,
    string Title,
    string Description,
    TimeSpan Duration,
    string Status
    ): ICommand<ResponseDto>;