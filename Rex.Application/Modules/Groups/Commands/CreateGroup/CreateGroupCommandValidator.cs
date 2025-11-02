using FluentValidation;

namespace Rex.Application.Modules.Groups.Commands.CreateGroup
{
    public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
    {
        public CreateGroupCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Group title is required.")
                .MaximumLength(100)
                .WithMessage("Group title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Group description is required.")
                .MaximumLength(500)
                .WithMessage("Group description cannot exceed 500 characters.");

            RuleFor(x => x.ProfilePhoto)
                .NotNull()
                .WithMessage("Profile photo is required.");

            RuleFor(x => x.Visibility)
                .IsInEnum()
                .WithMessage("Visibility must be a valid value.");
        }
    }
}