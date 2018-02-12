using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class EmailAttachmentEntity : AuditableEntity
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public string MimeType { get; set; }
        public string Size { get; set; }
        public string LanguageCode { get; set; }
    }
}
