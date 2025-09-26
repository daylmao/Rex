using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.Challenges.Commands.JoinChallenge;

public record JoinChallengeCommand(
    Guid ChallengeId,
    Guid UserId
    ): ICommand<ResponseDto>;