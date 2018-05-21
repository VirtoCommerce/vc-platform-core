using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Notifications
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string recipient, string subject, string message, params string[] parameters);
    }
}
