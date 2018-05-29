namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Settings of Email Protocol
    /// </summary>
    public class EmailSendingOptions
    {
        /// <summary>
        /// Email address of The Sender by default
        /// </summary>
        public string DefaultSender { get; set; }

        public string Gateway { get; set; }
    }

    public class SmtpSenderOptions
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class SendGridSenderOptions
    {
        public string ApiKey { get; set; }
    }
}
