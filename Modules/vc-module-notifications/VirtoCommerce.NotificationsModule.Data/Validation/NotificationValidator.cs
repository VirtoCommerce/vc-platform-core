using FluentValidation;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Data.Validation
{
    public class NotificationValidator : AbstractValidator<Notification>
    {
        public NotificationValidator()
        {
            RuleFor(n => n.Type).NotEmpty();
            RuleFor(n => n.Kind).NotEmpty();
        }
    }
}
