using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class EmailAttachmentEntity : AuditableEntity
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public string MimeType { get; set; }
        public string Size { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        [StringLength(10)]
        public string LanguageCode { get; set; }
    }
}
