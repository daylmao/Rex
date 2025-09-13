using FluentValidation;

namespace Rex.Application.Modules.User.Commands.RegisterUser;

public class RegisterUserValidation: AbstractValidator<RegisterUserCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png"];

    public RegisterUserValidation()
    {
        RuleFor(n => n.FirstName)
            .NotEmpty().WithMessage("Name is required");
        
        RuleFor(n => n.LastName)
            .NotEmpty().WithMessage("Lastname is required");
        
        RuleFor(n => n.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid");

        RuleFor(n => n.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .MaximumLength(30).WithMessage("Password cannot be longer than 30 characters");
        
        When(n => n.ProfilePhoto is not null, () =>
        {
            RuleFor(n => n.ProfilePhoto)
                .Must(n => n.Length > 0).WithMessage("Profile photo is required")
                .Must(n => _allowedExtensions.Contains(Path.GetExtension(n.FileName).ToLower()))
                .WithMessage("Profile photo extension is not supported")
                .Must(n => n.Length <= 5 * 1024 * 1024)
                .WithMessage("Profile photo size is too big");
        });
        
        When(n => n.CoverPhoto is not null, () =>
        {
            RuleFor(n => n.CoverPhoto)
                .Must(n => n.Length > 0).WithMessage("Cover Photo is required")
                .Must(n => _allowedExtensions.Contains(Path.GetExtension(n.FileName).ToLower()))
                .WithMessage("Cover Photo extension is not supported")
                .Must(n => n.Length <= 5 * 1024 * 1024)
                .WithMessage("Cover Photo size is too big");
        });

        
        RuleFor(n => n.Gender)
            .NotEmpty().WithMessage("Gender is required");
        
        RuleFor(n => n.Birthday)
            .NotEmpty().WithMessage("Birthday is required");
    }
}