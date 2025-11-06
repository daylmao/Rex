using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.UpdatePasswordByEmail;

public class UpdatePasswordByEmailValidation : AbstractValidator<UpdatePasswordByEmailCommand>
{
    public UpdatePasswordByEmailValidation()
    {
        RuleFor(n => n.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid")
            .MaximumLength(100).WithMessage("Email cannot be longer than 100 characters");
    }
}