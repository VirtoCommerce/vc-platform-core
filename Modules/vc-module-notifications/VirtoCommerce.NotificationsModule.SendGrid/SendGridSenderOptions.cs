using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.NotificationsModule.SendGrid
{
    public class SendGridSenderOptions
    {
        /// <summary>
        /// key of SendGrid API
        /// </summary>
        [Required]
        public string ApiKey { get; set; }
    }
}
