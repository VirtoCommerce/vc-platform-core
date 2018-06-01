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

        /// <summary>
        /// Gateway for sending email notifications
        /// </summary>
        public string Gateway { get; set; }
    }
}
