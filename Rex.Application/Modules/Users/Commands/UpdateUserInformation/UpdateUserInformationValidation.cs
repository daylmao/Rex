using FluentValidation;

namespace Rex.Application.Modules.Users.Commands.UpdateUserInformation;

public class UpdateUserInformationValidation : AbstractValidator<UpdateUserInformationCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png"];

    public UpdateUserInformationValidation()
    {
        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(n => n.Firstname)
            .NotEmpty().WithMessage("Firstname is required")
            .MinimumLength(2).WithMessage("Firstname must be at least 2 characters long")
            .MaximumLength(30).WithMessage("Firstname cannot be longer than 30 characters");

        RuleFor(n => n.Lastname)
            .NotEmpty().WithMessage("Lastname is required")
            .MinimumLength(2).WithMessage("Lastname must be at least 2 characters long")
            .MaximumLength(30).WithMessage("Lastname cannot be longer than 30 characters");

        When(n => !string.IsNullOrEmpty(n.Biography), () =>
        {
            RuleFor(n => n.Biography)
                .MaximumLength(200).WithMessage("Biography cannot be longer than 200 characters");
        });

        When(n => n.ProfilePhoto is not null, () =>
        {
            RuleFor(n => n.ProfilePhoto)
                .Must(n => n.Length > 0).WithMessage("Profile photo is required")
                .Must(n => _allowedExtensions.Contains(Path.GetExtension(n.FileName).ToLower()))
                .WithMessage("Profile photo extension is not supported")
                .Must(n => n.Length <= 5 * 1024 * 1024)
                .WithMessage("Profile photo size is too big");
        });
    }
}