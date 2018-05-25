namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Settings of Email Protocol
    /// </summary>
    public class EmailSendingOptions
    {
        /// <summary>
        /// Email address of The Sender
        /// </summary>
        public string DefaultSender { get; set; }

        /// <summary>
        /// Default smtp sending
        /// </summary>
        public SmtpOptions SmtpOptions { get; set; }

        /// <summary>
        /// SendGrid settings
        /// </summary>
        public SendGridOptions SendGridOptions { get; set; }
    }

    public class SmtpOptions
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class SendGridOptions
    {
        public string ApiKey { get; set; }
    }
}
