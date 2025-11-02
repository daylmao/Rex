using FluentValidation;

namespace Rex.Application.Modules.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadValidation : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadValidation()
    {
        RuleFor(n => n.NotificationId)
            .NotEmpty().WithMessage("NotificationId is required");
    }
}