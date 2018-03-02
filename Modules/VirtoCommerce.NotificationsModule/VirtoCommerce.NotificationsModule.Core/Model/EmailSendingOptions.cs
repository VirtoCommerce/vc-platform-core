namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class EmailSendingOptions
    {
        public SmtpOptions SmtpOptions { get; set; }
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
