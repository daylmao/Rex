using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.UpdateUsername;

public class UpdateUsernameValidation : AbstractValidator<UpdateUsernameCommand>
{
    public UpdateUsernameValidation()
    {
        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");
        
        RuleFor(n => n.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(5).WithMessage("Username must be at least 5 characters long")
            .MaximumLength(20).WithMessage("Username cannot be longer than 20 characters")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");
    }
}