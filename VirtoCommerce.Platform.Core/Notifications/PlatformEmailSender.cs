using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Notifications
{
    public class PlatformEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
}
