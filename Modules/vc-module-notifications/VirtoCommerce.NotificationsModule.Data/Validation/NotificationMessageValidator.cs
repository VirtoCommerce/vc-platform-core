using FluentValidation;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Data.Validation
{
    public class NotificationMessageValidator : AbstractValidator<NotificationMessage>
    {
        public NotificationMessageValidator()
        {
            RuleFor(m => m.NotificationType).NotEmpty();

            //special rules for derived types
            When(model => model.GetType() == typeof(EmailNotificationMessage), () =>
            {
                RuleFor(m => ((EmailNotificationMessage)m).From).NotEmpty().EmailAddress();
                RuleFor(m => ((EmailNotificationMessage)m).To).NotEmpty().EmailAddress();
                RuleFor(m => ((EmailNotificationMessage)m).Subject).NotEmpty();
                RuleFor(m => ((EmailNotificationMessage)m).Body).NotEmpty();
            });

            When(model => model.GetType() == typeof(SmsNotificationMessage), () =>
            {
                RuleFor(m => ((SmsNotificationMessage)m).Number).NotEmpty();
                RuleFor(m => ((SmsNotificationMessage)m).Message).NotEmpty();
            });

        }
    }
}
