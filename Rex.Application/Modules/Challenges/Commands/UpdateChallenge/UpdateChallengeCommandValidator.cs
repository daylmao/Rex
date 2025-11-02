using FluentValidation;
using Rex.Application.Modules.Challenges.Commands.UpdateChallenge;

namespace Rex.Application.Modules.Challenges.Commands.UpdateChallenge
{
    public class UpdateChallengeCommandValidator : AbstractValidator<UpdateChallengeCommand>
    {
        public UpdateChallengeCommandValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("GroupId is required.");

            RuleFor(x => x.ChallengeId)
                .NotEmpty()
                .WithMessage("ChallengeId is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.");

            RuleFor(x => x.Duration)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("Duration must be greater than zero.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status is invalid.");
        }
    }
}