using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.UpdatePassword;

public class UpdatePasswordValidation : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordValidation()
    {
        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(n => n.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required")
            .MinimumLength(6).WithMessage("Current password must be at least 6 characters long")
            .MaximumLength(30).WithMessage("Current password cannot be longer than 30 characters");

        RuleFor(n => n.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters long")
            .MaximumLength(30).WithMessage("New password cannot be longer than 30 characters")
            .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("New password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character");

        RuleFor(n => n)
            .Must(n => n.CurrentPassword != n.NewPassword)
            .WithMessage("New password cannot be the same as the current password");
    }
}