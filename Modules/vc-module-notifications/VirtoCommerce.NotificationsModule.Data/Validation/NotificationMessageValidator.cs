using FluentValidation;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Data.Validation
{
    public class NotificationMessageValidator : AbstractValidator<NotificationMessage>
    {
        public NotificationMessageValidator()
        {
            RuleFor(m => m.NotificationId).NotEmpty();
            RuleFor(m => m.NotificationType).NotEmpty();
        }
    }
}
