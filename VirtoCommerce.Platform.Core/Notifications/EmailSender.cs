using System;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Notifications
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string recipient, string subject, string message, params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
