using System;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Notifications
{
    public class DefaultEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
}
