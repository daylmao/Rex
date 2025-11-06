using FluentValidation;

namespace Rex.Application.Modules.Challenges.Commands.JoinChallenge
{
    public class JoinChallengeCommandValidator : AbstractValidator<JoinChallengeCommand>
    {
        public JoinChallengeCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.ChallengeId)
                .NotEmpty()
                .WithMessage("ChallengeId is required.");
        }
    }
}