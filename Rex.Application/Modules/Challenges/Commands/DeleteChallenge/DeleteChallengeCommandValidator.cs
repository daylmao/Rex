using FluentValidation;

namespace Rex.Application.Modules.Challenges.Commands.DeleteChallenge
{
    public class DeleteChallengeCommandValidator : AbstractValidator<DeleteChallengeCommand>
    {
        public DeleteChallengeCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("GroupId is required.");

            RuleFor(x => x.ChallengeId)
                .NotEmpty()
                .WithMessage("ChallengeId is required.");
        }
    }
}