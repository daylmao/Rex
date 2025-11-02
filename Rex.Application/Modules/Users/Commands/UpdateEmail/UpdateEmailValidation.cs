using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.UpdateEmail;

public class UpdateEmailValidation : AbstractValidator<UpdateEmailCommand>
{
    public UpdateEmailValidation()
    {
        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(n => n.Email)
            .NotEmpty().WithMessage("Current email is required")
            .EmailAddress().WithMessage("Current email format is invalid")
            .MaximumLength(100).WithMessage("Current email cannot be longer than 100 characters");

        RuleFor(n => n.NewEmail)
            .NotEmpty().WithMessage("New email is required")
            .EmailAddress().WithMessage("New email format is invalid")
            .MaximumLength(100).WithMessage("New email cannot be longer than 100 characters");

        RuleFor(n => n)
            .Must(n => n.Email != n.NewEmail)
            .WithMessage("New email cannot be the same as the current email");
    }
}