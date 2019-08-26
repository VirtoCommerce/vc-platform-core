using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Settings of SMS Protocol
    /// </summary>
    public class SmsSendingOptions
    {
        /// <summary>
        /// SMS address of The Sender by default
        /// </summary>
        [Phone]
        public string SmsDefaultSender { get; set; }

        /// <summary>
        /// Gateway for sending SMS notifications
        /// </summary>
        [Required]
        public string SmsGateway { get; set; }
    }
}
