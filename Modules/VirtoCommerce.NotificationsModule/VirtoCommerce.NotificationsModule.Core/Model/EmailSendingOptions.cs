namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class EmailSendingOptions
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
    }
}
