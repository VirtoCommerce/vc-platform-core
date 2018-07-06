using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.SitemapsModule.Core.Models.PushNotifications
{
    public class SitemapDownloadNotification : PushNotification
    {
        public SitemapDownloadNotification(string creator)
            : base(creator)
        {
            NotifyType = "SitemapDownload";
            Errors = new List<string>();
        }

        [JsonProperty("finished")]
        public DateTime? Finished { get; set; }

        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }

        [JsonProperty("processedCount")]
        public long ProcessedCount { get; set; }

        [JsonProperty("errorCount")]
        public long ErrorCount
        {
            get
            {
                return Errors != null ? Errors.Count : 0;
            }
        }

        [JsonProperty("errors")]
        public ICollection<string> Errors { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }
    }
}
