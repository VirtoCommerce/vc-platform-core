using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Notifications
{
    public class PlatformEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string to, string type, string languageCode, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
