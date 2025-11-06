using FluentValidation;

namespace Rex.Application.Modules.Groups.Commands.UpdateGroup
{
    public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
    {
        public UpdateGroupCommandValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty()
                .WithMessage("GroupId is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .When(x => x.Title is not null)
                .WithMessage("Title cannot be empty if provided.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .When(x => x.Description is not null)
                .WithMessage("Description cannot be empty if provided.");

            RuleFor(x => x.Visibility)
                .NotNull()
                .When(x => x.Visibility is not null)
                .WithMessage("Visibility cannot be null if provided.");
        }
    }
}