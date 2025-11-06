using FluentValidation;

namespace Rex.Application.Modules.Challenges.Commands.CreateChallenge;

public class CreateChallengeValidation : AbstractValidator<CreateChallengeCommand>
{
    public CreateChallengeValidation()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(c => c.GroupId)
            .NotEmpty().WithMessage("GroupId is required.");

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(c => c.Duration)
            .NotEmpty().WithMessage("Duration is required.")
            .Must(d => d > TimeSpan.Zero).WithMessage("Duration must be greater than zero.");

        RuleFor(c => c.CoverPhoto)
            .NotNull().WithMessage("Cover photo is required.")
            .Must(file => file.Length > 0).WithMessage("Cover photo cannot be empty.")
            .Must(file => file.ContentType.StartsWith("image/")).WithMessage("Cover photo must be an image file.");
    }
}