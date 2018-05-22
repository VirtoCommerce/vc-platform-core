using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Notifications
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string type, string languageCode, Dictionary<string, object> parameters);
    }
}
