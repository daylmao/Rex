using FluentValidation;

namespace Rex.Application.Modules.Posts.Commands.CreatePost;

public class CreatePostValidation : AbstractValidator<CreatePostCommand>
{
    public CreatePostValidation()
    {
        RuleFor(n => n.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(n => n.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(n => n.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(150).WithMessage("Title must not exceed 150 characters");

        RuleFor(n => n.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        When(n => n.Files != null && n.Files.Any(), () =>
        {
            RuleForEach(n => n.Files).ChildRules(file =>
            {
                file.RuleFor(f => f.Length)
                    .LessThanOrEqualTo(5 * 1024 * 1024) // 5 MB
                    .WithMessage("Each file must be less than 5 MB");

                file.RuleFor(f => f.ContentType)
                    .Must(type => type.StartsWith("image/") || type.StartsWith("video/"))
                    .WithMessage("Only image or video files are allowed");
            });
        });
    }
}