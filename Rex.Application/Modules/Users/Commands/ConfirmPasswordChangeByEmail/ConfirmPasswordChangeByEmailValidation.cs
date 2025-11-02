using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.ConfirmPasswordChangeByEmail;

public class ConfirmPasswordChangeByEmailValidation : AbstractValidator<ConfirmPasswordChangeByEmailCommand>
{
    public ConfirmPasswordChangeByEmailValidation()
    {
        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(n => n.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(6).WithMessage("Code must be exactly 6 characters long");

        RuleFor(n => n.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters long")
            .MaximumLength(30).WithMessage("New password cannot be longer than 30 characters")
            .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("New password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character");
    }
}