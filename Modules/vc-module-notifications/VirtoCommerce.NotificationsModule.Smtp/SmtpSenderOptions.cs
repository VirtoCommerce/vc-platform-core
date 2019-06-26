using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.NotificationsModule.Smtp
{
    /// <summary>
    /// Smtp protocol
    /// </summary>
    public class SmtpSenderOptions
    {
        /// <summary>
        /// Server of Sending
        /// </summary>
        [Url]
        public string SmtpServer { get; set; }

        /// <summary>
        /// Port of Server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Login of Sending Server
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Password of Sending Server
        /// </summary>
        public string Password { get; set; }
    }
}
